using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace DockerSlaveManager.Util
{
    public static class ZipExtensions
    {
        public static async Task WriteStringToFileAsync(this ZipArchive archive, string filename, string data)
        {
            var file = archive.CreateEntry(filename).Open();
            using (var stream = new StreamWriter(file))
            {
                await stream.WriteAsync(data);
                stream.Flush();
            }
            file.Close();
        }
    }
}
