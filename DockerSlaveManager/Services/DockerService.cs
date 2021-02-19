using Docker.DotNet;
using Docker.DotNet.Models;
using DockerSlaveManager.Models.Multipart;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DockerSlaveManager.Util;
using System.Net.Http;
using System.Text;

namespace DockerSlaveManager.Services
{

    public class DockerService : BackgroundService
    {
        private DockerClient client;
        private Timer timer;
        private Config config;

        private IBackgroundTaskQueue TaskQueue;
        private ILogger logger;

        private Dictionary<string, QueueItem> queueStatus;
        private HttpClient httpClient;

        public DockerService(IOptions<Config> config, IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
        {
            this.config = config.Value;
            this.TaskQueue = taskQueue;
            this.logger = loggerFactory.CreateLogger<DockerService>();
            this.queueStatus = new Dictionary<string, QueueItem>();
            this.httpClient = new HttpClient();
        }

        public IEnumerable<QueueItem> Queue()
        {
            return this.queueStatus.Values;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting Docker Service");
            client = new DockerClientConfiguration(new Uri(config.ConnectionString)).CreateClient();

            var imageList = await client.Images.ListImagesAsync(new ImagesListParameters()
            {
                All = true
            });
            Console.WriteLine($"{imageList.Count} images found");

            timer = new Timer(CheckContainers, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);
                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex,"Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        private void CheckContainers(object state)
        {
            //remove items that are done after 5 minutes
            queueStatus = queueStatus
                .Where(kv => !(kv.Value.status == Status.Done && kv.Value.finishTime.AddMinutes(5) < DateTime.Now))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }

        public string RunContainerBackground(RunConfig runConfig)
        {
            Guid id = Guid.NewGuid();
            queueStatus[id.ToString()] = new QueueItem() { Id = id.ToString() };
            TaskQueue.QueueBackgroundWorkItem(async token => await RunContainer(runConfig, id));
            return id.ToString();
        }

        public QueueItem GetRunStatus(string id)
        {
            if (queueStatus.ContainsKey(id))
                return queueStatus[id];
            else
                return null;
        }

        public void CleanRun(string id)
        {

            var localPath = Path.Combine(config.SharedMountLocal, id.ToString());
            Directory.Delete(localPath, true);
        }


        private async Task RunContainer(RunConfig runConfig, Guid id)
        {
            queueStatus[id.ToString()].status = Status.Running;
            await UpdateImage(runConfig.Image);
            //TODO: should the imagelist be used?
            var imageList = await client.Images.ListImagesAsync(new ImagesListParameters()
            {
                All = true,
                MatchName = runConfig.Image
            });

            var image = imageList.Where(img => img.RepoTags != null && img.RepoTags.Contains(runConfig.Image + ":latest")).FirstOrDefault();
            if(image == null)
            {
                Console.WriteLine("Error finding image: ");
                foreach (var img in imageList)
                {
                    Console.WriteLine("Id: " + img.ID);
                    if (img.RepoTags != null)
                        Console.WriteLine("Tags: " + string.Join(',', img.RepoTags));
                    else
                        Console.WriteLine("Tags: null");
                }
            }

            var localPath = Path.Combine(config.SharedMountLocal, id.ToString());
            var srcPath = config.SharedMount + "/" + id.ToString();
            Directory.CreateDirectory(localPath);

            var localPathOut = Path.Combine(localPath, "out");
            var srcPathOut = srcPath + "/out";
            Directory.CreateDirectory(localPathOut);

            var createParams = new CreateContainerParameters()
            {
                Image = image.ID,
                Name = runConfig.Image.Replace("/", "_") + "_" + id,
                
                HostConfig = new HostConfig()
                {
                    Mounts = new List<Mount>()
                    {
                        new Mount()
                        {
                            Type = "bind",
                            Source = srcPathOut,
                            Target = runConfig.OutPath
                        }
                    },
                },
            };

            if(runConfig.Environment != null && runConfig.Environment.Count > 0)
            {
                createParams.Env = runConfig.Environment.Select(kv => $"{kv.Key}={kv.Value}").ToList();
            }

            foreach(var m in runConfig.Mounts)
            {
                string randomName = Guid.NewGuid().ToString("n").Substring(0, 16);
                var localPathM = Path.Combine(localPath, randomName);
                var srcPathM = srcPath + "/" + randomName;
                Directory.CreateDirectory(localPathM);
                var original = m.Value;
                Console.WriteLine($"Downloading {original}");
                //TODO: cache downloads
                var httpResponse = await httpClient.GetAsync(original);
                if (!httpResponse.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Could not download {original}");
                    continue;
                }
                byte[] data = await httpResponse.Content.ReadAsByteArrayAsync();
                MemoryStream ms = new MemoryStream(data);
                Console.WriteLine($"unzipping {original}");
                using (var zipFile = new ZipArchive(ms))
                {
                    foreach (var entry in zipFile.Entries)
                    {
                        if (entry.FullName.Contains("..")) //TODO: bad zip prevention, improve
                            continue;
                        if (entry.FullName.EndsWith("/"))
                            Directory.CreateDirectory(Path.Combine(localPathM, entry.FullName));
                        else
                        {
                            Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(localPathM, entry.FullName)));
                            entry.ExtractToFile(Path.Combine(localPathM, entry.FullName));
                        }
                    }
                }

                createParams.HostConfig.Mounts.Add(new Mount()
                {
                    Type = "bind",
                    Source = srcPathM,
                    Target = m.Key
                });
            }
            

            var response = await client.Containers.CreateContainerAsync(createParams);


            Console.WriteLine($"Created container {response.ID}");


            queueStatus[id.ToString()].runTime = DateTime.Now;
            Console.WriteLine($"Running container {response.ID}");
            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
            Console.WriteLine($"Waiting for container {response.ID} to finish running");

            CancellationToken token = CancellationToken.None;
            var stream = await client.Containers.AttachContainerAsync(response.ID, true, new ContainerAttachParameters() { Stdout = true, Stream = true });
            (string stdout, string stderr) p = await stream.ReadOutputToEndAsync(token);

            queueStatus[id.ToString()].stderr = p.stderr;
            queueStatus[id.ToString()].stdout = p.stdout;
            queueStatus[id.ToString()].finishTime = DateTime.Now;
            queueStatus[id.ToString()].status = Status.Done;

            Console.WriteLine($"Removing container {response.ID}");
            await client.Containers.RemoveContainerAsync(response.ID, new ContainerRemoveParameters());

            var zipfile = Path.Combine(config.SharedMountLocal, id.ToString() + ".zip");

            using (FileStream zipToOpen = new FileStream(zipfile, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
                {
                    await archive.WriteStringToFileAsync("stdout.txt", queueStatus[id.ToString()].stdout);
                    await archive.WriteStringToFileAsync("stderr.txt", queueStatus[id.ToString()].stderr);

                    foreach (var filePath in Directory.EnumerateFiles(localPathOut, "*.*", SearchOption.AllDirectories))
                    {
                        string fileName = Path.GetFileName(filePath);
                        var entry = archive.CreateEntryFromFile(filePath, fileName);
                    }
                }
            }
            Directory.Delete(localPath, true);

            if (runConfig.CallbackUrl != null)
            {
                var client = new HttpClient();
                Console.WriteLine("Posting results to " + runConfig.CallbackUrl + "/?id=" + id);
                await client.PostAsync(runConfig.CallbackUrl + "/?id=" + id, new ByteArrayContent(File.ReadAllBytes(zipfile)));
            }
            Console.WriteLine($"Done running container {response.ID}");
        }

        private async Task UpdateImage(string image)
        {
            await client.Images.CreateImageAsync(new ImagesCreateParameters()
            {
                FromImage = image,
                Tag = "latest",
                Repo = "https://hub.docker.com/"

            }, new AuthConfig(), new Progress<JSONMessage>(m => Console.WriteLine(m.Status)));
        }


        public new Task StopAsync(CancellationToken cancellationToken)
        {
            base.StopAsync(cancellationToken);
            Console.WriteLine("Stopping Docker Service");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
