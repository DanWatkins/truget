using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NuGet.Packaging;

namespace TruGet
{
    public class PackageHarvester
    {
        public async Task RunAsync(string packageFilepath, string outputPath)
        {
            var packageFilename = new FileInfo(packageFilepath).Name;
            if (_packagesChecked.Contains(packageFilename))
                return;
            _packagesChecked.Add(packageFilename);

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

            foreach (var group in groups)
            {
                foreach (var package in group.Packages)
                {
                    var packageDependency = new PackageDependency(
                        package.Id,
                        package.VersionRange);

                    try
                    {
                        var outputFilepath = await new PackageDownloader().DownloadIfNeededAsync(
                            packageDependency,
                            outputPath);

                        await RunAsync(outputFilepath, outputPath);
                    }
                    catch (WebException wex)
                    {
                        Console.WriteLine($"ERROR: {wex.Message}");
                    }
                }
            }
        }

        private readonly List<string> _packagesChecked = new List<string>();
    }
}