using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DockerSlaveManager.Services
{
    public class DockerService : IHostedService
    {
        private DockerClient client;
        private IList<ImagesListResponse> imageList;
        private Timer timer;
        private Config config;

        public DockerService(IOptions<Config> config)
        {
            this.config = config.Value;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Starting Docker Service");
            client = new DockerClientConfiguration(new Uri(config.ConnectionString)).CreateClient();

            this.imageList = await client.Images.ListImagesAsync(new ImagesListParameters()
            {
                All = true
            });
            Console.WriteLine($"{this.imageList.Count} images found");

            timer = new Timer(CheckContainers, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private void CheckContainers(object state)
        {
            Console.WriteLine("checking containers");

        }

        public Task updateContainers()
        {
            return Task.CompletedTask;
        }

        public async Task RunContainer(string image)
        {
            var response = await client.Containers.CreateContainerAsync(new CreateContainerParameters()
            {
                Image = "sha256:8c7c2fb833d0893473ccd2e41cabf7a0bc36a340a20dd62ab8aec278e8c717a8",
                Name = "blabla"
            });


            await client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());

            CancellationToken token;

            var stream = await client.Containers.AttachContainerAsync(response.ID, true, new ContainerAttachParameters() { Stdout = true, Stream = true });

            (string stdout, string stderr) p = await stream.ReadOutputToEndAsync(token);
            Console.WriteLine(p.stdout);
            Console.WriteLine(p.stderr);

            await client.Containers.RemoveContainerAsync(response.ID, new ContainerRemoveParameters());
        }


        public Task StopAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Stopping Docker Service");
            timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}
