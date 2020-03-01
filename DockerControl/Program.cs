using Docker.DotNet;
using Docker.DotNet.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DockerControl
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            DockerClient client = new DockerClientConfiguration(new Uri("tcp://nas.borf.nl:2375")).CreateClient();


            await client.Images.CreateImageAsync(new ImagesCreateParameters()
            {
                FromImage = "proglet/projectparser-intellij-edutools",
                Tag = "latest",
                Repo = "https://hub.docker.com/"

            }, new AuthConfig(), new Progress<JSONMessage>());



            Console.WriteLine("Volumes");
            var list = await client.Volumes.ListAsync();
            foreach(var x in list.Volumes)
            {
                Console.WriteLine(x.Name);
            }
            
            var images = await client.Images.ListImagesAsync(new ImagesListParameters()
            {
                
            });

            Console.WriteLine("Images: ");
            foreach(var x in images)
            {
                Console.WriteLine($"Id: {x.ID}");
                Console.WriteLine($"Parent: {x.ParentID}");
                Console.WriteLine($"Containers: {x.Containers}");
                if(x.RepoDigests != null)
                    Console.WriteLine($"RepoDigests: {String.Join(",", x.RepoDigests)}");
                if(x.RepoTags != null)
                    Console.WriteLine($"RepoTags: {String.Join(",", x.RepoTags)}");
            }
            Console.WriteLine();



            IList<ContainerListResponse> containers = await client.Containers.ListContainersAsync(
                new ContainersListParameters()
                {
                    Limit = 100,
            });

            Console.WriteLine("Containers: ");
            foreach (var x in containers)
            {
                Console.WriteLine(x.ToString());
                Console.WriteLine($"Image: {x.Image}");
                Console.WriteLine($"ImageID: {x.ImageID}");
                Console.WriteLine($"Names: {String.Join(", ", x.Names)}");
                Console.WriteLine($"State: {x.State}");
                Console.WriteLine($"Status: {x.Status}");

                Console.WriteLine();
            }

            Console.ReadLine();
         }
    }
}
