using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NuGet.Packaging;

namespace TruGet
{
    public static class PackageHarvester
    {
        public static async Task RunAsync(string packageFilepath, string outputPath)
        {
            using var zipFile = ZipFile.OpenRead(packageFilepath);

            var nuspec = zipFile.Entries
                .SingleOrDefault(e => e.FullName.EndsWith(".nuspec"));

            if (nuspec == null)
            {
                Console.WriteLine($"ERROR: Package {zipFile.ToString()} does not contain a nuspec file.");
                return;
            }

            Directory.CreateDirectory(outputPath);

            await using var stream = nuspec.Open();
            var reader = new NuspecReader(stream);
            var groups = reader.GetDependencyGroups().ToList();

            Console.WriteLine($"Downloading depenencies for {packageFilepath}");

            foreach (var group in groups)
            {
                foreach (var package in group.Packages)
                {
                    var version = package.VersionRange.ToShortString();
                    var url = $"https://www.nuget.org/api/v2/package/{package.Id}/{version}";
                    var filename = $"{package.Id}.{version}.nuspec";
                    var outputFilepath = Path.Combine(outputPath, filename);

                    if (File.Exists(outputFilepath))
                        continue;

                    try
                    {
                        Console.WriteLine($"{url}");
                        await new WebClient().DownloadFileTaskAsync(url, outputFilepath);
                    }
                    catch (WebException wex)
                    {
                        Console.WriteLine($"ERROR: {wex.Message}");
                        return;
                    }

                    await PackageHarvester.RunAsync(outputFilepath, outputPath);
                }
            }
        }
    }
}