using System;
using System.Threading.Tasks;

namespace TruGet
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            const string repositoryPath = @"C:\Users\dwatk\code\realityfocus-forks\Avalonia";
            const string outputPath = @"C:\Users\dwatk\Desktop\avalonia-dependencies";
            
            var dependencies = await new PackageDependencyIdentifier().RunAsync(repositoryPath);

            foreach (var dependency in dependencies)
            {
                Console.WriteLine($"[{dependency.Id}-{dependency.Version}]");

                var path = await new PackageDownloader().DownloadIfNeededAsync(dependency, outputPath);
                await new PackageHarvester().RunAsync(path, outputPath);
            }
            
            Console.WriteLine("DONE");
        }
    }
}
