using CoreDataORM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IDockerService
    {
        void RegisterSlaveManager(string url);
        void RunContainer(string image, Dictionary<string, string> environment, Delegate callback);
        void Callback(string id, byte[] data, DataContext context);
    }
    public class DockerService : IDockerService
    {
        private List<string> urls = new List<string>();
        private static readonly HttpClient client = new HttpClient();
        private Dictionary<string, Delegate> callbacks = new Dictionary<string, Delegate>();


        public void RegisterSlaveManager(string url)
        {
            if(!urls.Contains(url))
                urls.Add(url);
        }

        public async void RunContainer(string image, Dictionary<string, string> environment, Delegate callback)
        {
            var values = new Dictionary<string, string>
            {
                { "Image", image },
                { "CallbackUrl", "http://localhost:5000/api/slaves/callback" },
                { "Environment[test]", "test2" },
                { "ZipOverlay", "" }
            };

            var content = new FormUrlEncodedContent(values);
            var response = await client.PostAsync(urls[0] + "/api/docker/run", content);
            var responseString = await response.Content.ReadAsStringAsync();
            callbacks[responseString] = callback;
        }

        public void Callback(string id, byte[] data, DataContext dataContext)
        {
            if (callbacks.ContainsKey(id))
            {
                //deligate voodoo magic, look up the parameters of the delegate entered, and fill them in
                Delegate callback = callbacks[id];
                var parameters = callback.Method.GetParameters();
                object[] p = new object[parameters.Length];
                for(int i = 0; i < parameters.Length; i++)
                {
                    var type = parameters[i].ParameterType;
                    if (type == typeof(byte[]))
                        p[i] = data;
                    if (type == typeof(DataContext))
                        p[i] = dataContext;
                }
                callback.DynamicInvoke(p);
            }
        }
    }
}
