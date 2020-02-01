using System;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;
using NuGet.Packaging;

namespace TruGet
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            const string path = @"C:\Users\dwatk\Desktop\moq.4.7.99.nupkg";
            const string output = @"C:\Users\dwatk\Desktop\extracted\";

            using var zipFile = ZipFile.OpenRead(path);

            var nuspec = zipFile.Entries
                .SingleOrDefault(e => e.FullName.EndsWith(".nuspec"));

            if (nuspec == null)
            {
                Console.WriteLine($"ERROR: Package {zipFile.ToString()} does not contain a nuspec file.");
                return;
            }

            await using var stream = nuspec.Open();
            var reader = new NuspecReader(stream);
            var groups = reader.GetDependencyGroups().ToList();

            Console.WriteLine($"Downloading depenencies for {zipFile}");
            
            foreach (var group in groups)
            {
                Console.WriteLine($"TargetFramework:{group.TargetFramework}");
                foreach (var package in group.Packages)
                {
                    var url = $"https://www.nuget.org/api/v2/package/{package.Id}/{package.VersionRange.OriginalString}";
                    Console.WriteLine($"{url}");
                }
            }
        }
    }
}
