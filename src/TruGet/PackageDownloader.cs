using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using NuGet.Common;
using NuGet.Configuration;
using NuGet.Protocol.Core.Types;
using NuGet.Protocol.VisualStudio;

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
                Console.WriteLine($" {filename} GET {url}");
                await new WebClient().DownloadFileTaskAsync(url, outputFilepath);
            }

            return outputFilepath;
        }

        public async Task<string> DownloadLatestVersionIfNeededAsync(string packageId, string outputPath)
        {
            var visStudio = Repository.Provider.GetVisualStudio();
            var packageSource = new PackageSource("https://api.nuget.org/v3/index.json");
            var sourceRepository = Repository.CreateSource(visStudio, packageSource);
            var packageSearchResource = await sourceRepository.GetResourceAsync<PackageSearchResource>();

            var results = (await packageSearchResource.SearchAsync(
                    packageId,
                    new SearchFilter(false),
                    0,
                    1,
                    NullLogger.Instance,
                    CancellationToken.None))
                .ToList();

            if (results.Count == 0)
            {
                Console.WriteLine($"ERROR: Package {packageId} was not found from source {packageSource}.");
                return null;
            }

            var packageMetadata = results[0];

            if (!string.Equals(packageMetadata.Identity.Id, packageId, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine(
                    $"ERROR: Package {packageId} was not found from source {packageSource}. Best match was {packageMetadata.Identity.Id}.");
                return null;
            }

            return await DownloadIfNeededAsync(
                new PackageDependency(packageMetadata.Identity.Id, packageMetadata.Identity.Version.OriginalVersion),
                outputPath);
        }
    }
}