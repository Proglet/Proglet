using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DockerSlaveManager.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DockerSlaveManager
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Config>(Configuration.GetSection("Docker"));
            services.AddControllers();
            services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
            services.AddSingleton<IHostedService, DockerService>();
            services.AddSingleton<DockerService>(sp => sp.GetServices<IHostedService>().ToList().Find(x => x.GetType() == typeof(DockerService)) as DockerService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (!Directory.Exists("data"))
                Directory.CreateDirectory("data");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            for (int i = 0; i < 5; i++)
            {
                //register self at API host
                string ApiUrl = Configuration.GetValue<string>("ApiUrl");
                string MyUrl = Configuration.GetValue<string>("MyUrl");
                string Auth = Configuration.GetValue<string>("Auth");

                var client = new HttpClient();
                string parameters = JsonSerializer.Serialize(new
                {
                    Url = MyUrl,
                    Auth = Auth,
                });
                try
                {
                    var response = await client.PostAsync(ApiUrl + "/api/slaves/register", new StringContent(parameters, System.Text.Encoding.UTF8, "application/json"));
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine("API denied access, retrying");
                    }
                    else
                    {
                        Console.WriteLine("Registered and confirmed at API host");
                        return;
                    }
                }
                catch (HttpRequestException)
                {
                    Console.WriteLine("Could not connect to API, closing");
                }
                await Task.Delay(2000);
            }
            Program.Shutdown();
        }
    }
}
