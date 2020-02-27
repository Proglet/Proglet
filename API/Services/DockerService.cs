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
        void RunContainer(string image, Dictionary<string, string> environment, Action<byte[]> callback);
        void Callback(string id, byte[] data);
    }
    public class DockerService : IDockerService
    {
        private List<string> urls = new List<string>();
        private static readonly HttpClient client = new HttpClient();
        private Dictionary<string, Action<byte[]>> callbacks = new Dictionary<string, Action<byte[]>>();

        public void RegisterSlaveManager(string url)
        {
            if(!urls.Contains(url))
                urls.Add(url);
        }

        public async void RunContainer(string image, Dictionary<string, string> environment, Action<byte[]> callback)
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

        public void Callback(string id, byte[] data)
        {
            if (callbacks.ContainsKey(id))
                callbacks[id](data);
        }
    }
}
