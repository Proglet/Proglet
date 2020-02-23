using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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


        public DockerService(IOptions<Config> config, IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
        {
            this.config = config.Value;
            this.TaskQueue = taskQueue;
            this.logger = loggerFactory.CreateLogger<DockerService>();
            this.queueStatus = new Dictionary<string, QueueItem>();
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
            //await RunContainer("proglet/projectparser-intellij-edutools");
        }

        private void CheckContainers(object state)
        {
            Console.WriteLine($"checking containers, {queueStatus.Count} queued");
            //TODO: test if items in queue are done
        }

 

        public string RunContainerBackground(string image)
        {
            Guid id = Guid.NewGuid();
            queueStatus[id.ToString()] = new QueueItem();
            TaskQueue.QueueBackgroundWorkItem(async token => await RunContainer(image, id));
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


        private async Task RunContainer(string image, Guid id)
        {
            queueStatus[id.ToString()].status = Status.Running;
            await UpdateImage(image);
            //TODO: should the imagelist be used?
            var imageList = await client.Images.ListImagesAsync(new ImagesListParameters()
            {
                All = true,
                MatchName = image
            });



            var localPath = Path.Combine(config.SharedMountLocal, id.ToString());
            var srcPath = Path.Combine(config.SharedMount, id.ToString());
            Directory.CreateDirectory(localPath);
            var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = imageList[0].ID,
                Name = image.Replace("/", "_") + "_ASD",
                HostConfig = new HostConfig()
                {
                    Mounts = new List<Mount>()
                    {
                        new Mount()
                        {
                            Type = "bind",
                            Source = srcPath,
                            Target = "/app/out"
                        }
                    }
                },
            });
            Console.WriteLine($"Created container {response.ID}");


            Console.WriteLine($"Running container {response.ID}");
            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());
            Console.WriteLine($"Waiting for container {response.ID} to finish running");
            await client.Containers.WaitContainerAsync(response.ID);

            string[] files = Directory.GetFiles(localPath);
            foreach(var filename in files)
            {
                Console.WriteLine(filename);
                Console.WriteLine(File.ReadAllText(filename));
            }

            queueStatus[id.ToString()].status = Status.Done;

            //CancellationToken token;

            //var stream = await client.Containers.AttachContainerAsync(response.ID, true, new ContainerAttachParameters() { Stdout = true, Stream = true });

            //(string stdout, string stderr) p = await stream.ReadOutputToEndAsync(token);
            //Console.WriteLine(p.stdout);
            //Console.WriteLine(p.stderr);
            Console.WriteLine($"Removing container {response.ID}");
            await client.Containers.RemoveContainerAsync(response.ID, new ContainerRemoveParameters());
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
