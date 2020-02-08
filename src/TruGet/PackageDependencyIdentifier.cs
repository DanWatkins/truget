using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TruGet
{
    public class PackageDependencyIdentifier
    {
        public async Task<IEnumerable<PackageDependency>> RunAsync(string directoryPath)
        {
            var files = Directory.EnumerateFiles(directoryPath);

            var packageReferences = await Task.WhenAll(files
                .Where(IsPackageReferenceFile)
                .Select(ProcessPackageReferenceFileAsync));

            var packagesConfig = await Task.WhenAll(files
                .Where(IsPackagesConfigFile)
                .Select(ProcessPackageConfigFile));

            var subdirs = await Task.WhenAll(Directory
                .EnumerateDirectories(directoryPath)
                .Select(RunAsync));

            return packageReferences
                .Concat(packagesConfig)
                .Concat(subdirs)
                .SelectMany(x => x);
        }

        private bool IsPackageReferenceFile(string file)
        {
            return file.EndsWith(".csproj") || file.EndsWith(".props") || file.EndsWith(".proj");
        }

        private bool IsPackagesConfigFile(string file)
        {
            return file.EndsWith("packages.config");
        }

        private async Task<IEnumerable<PackageDependency>> ProcessPackageReferenceFileAsync(string file)
        {
            var lines = await File.ReadAllLinesAsync(file);

            return lines
                .Where(l => l.Contains("<PackageReference"))
                .Select(l =>
                {
                    var includeMatches = Regex.Matches(l, "Include=\"([^\"]*)\"");
                    var id = includeMatches[0].Groups[1].ToString();

                    var versionMatches = Regex.Matches(l, "Version=\"([^\"]*)\"");
                    var version = versionMatches[0].Groups[1].ToString();

                    return new PackageDependency(id, version);
                });
        }

        private async Task<IEnumerable<PackageDependency>> ProcessPackageConfigFile(string file)
        {
            var lines = await File.ReadAllLinesAsync(file);

            return lines
                .Where(l => l.StartsWith("<package "))
                .Select(l =>
                {
                    var includeMatches = Regex.Matches(l, "id=\"([^\"]*)\"");
                    var id = includeMatches[0].Groups[1].ToString();

                    var versionMatches = Regex.Matches(l, "version=\"([^\"]*)\"");
                    var version = versionMatches[0].Groups[1].ToString();

                    return new PackageDependency(id, version);
                });
        }
    }
}