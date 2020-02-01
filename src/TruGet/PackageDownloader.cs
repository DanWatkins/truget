using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace TruGet
{
    public class PackageDownloader
    {
        public async Task<string> DownloadIfNeededAsync(PackageDependency package, string outputPath)
        {
            var url = $"https://www.nuget.org/api/v2/package/{package.Id}/{package.Version}";
            var filename = $"{package.Id}.{package.Version}.nupkg";
            var outputFilepath = Path.Combine(outputPath, filename);

            if (!File.Exists(outputFilepath))
            {
                Console.WriteLine($"{filename} GET {url}");
                await new WebClient().DownloadFileTaskAsync(url, outputFilepath);
            }

            return outputFilepath;
        }
    }
}