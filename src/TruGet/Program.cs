using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TruGet
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            const string repositoryPath = @"C:\Users\dwatk\Code\RealityFocusForks\Avalonia\";
            const string outputPath = @"C:\Users\dwatk\Code\RealityFocusForks\Avalonia\.truget\";

            var dependencies = await new PackageDependencyIdentifier().RunAsync(repositoryPath);

            foreach (var dependency in dependencies)
            {
                Console.WriteLine($"[{dependency.Id}-{dependency.Version}]");

                var path = await new PackageDownloader().DownloadIfNeededAsync(dependency, outputPath);
                await new PackageHarvester().RunAsync(path, outputPath);
            }

            var implicitDepenencies = new[]
            {
                ("Microsoft.Build.Traversal",""),
                ("MSBuild.Sdk.Extras", ""),
                ("Microsoft.NETCore.Targets", "1.1.1"),
                ("Microsoft.NETCore.App", "2.2.8"),
                ("Microsoft.NETCore.App", "2.1.15"),
                ("Microsoft.NETCore.App", "2.0.9"),
                ("Microsoft.NETCore.App", "2.0.0"),
                ("System.Private.Uri", ""),
                ("runtime.win7.System.Private.Uri", ""),
                ("runtime.any.System.Collections", ""),
                ("runtime.any.System.Collections", ""),
                ("runtime.any.System.Diagnostics.Tracing", ""),
                ("runtime.any.System.Globalization", ""),
                ("runtime.any.System.IO", ""),
                ("runtime.any.System.Reflection", ""),
                ("runtime.any.System.Reflection.Extensions", ""),
                ("runtime.any.System.Reflection.Primitives", ""),
                ("runtime.any.System.Resources.ResourceManager", ""),
                ("runtime.any.System.Runtime", ""),
                ("runtime.any.System.Runtime.Handles", ""),
                ("runtime.any.System.Runtime.InteropServices", ""),
                ("runtime.any.System.Text.Encoding", ""),
                ("runtime.any.System.Threading.Tasks", ""),
                ("runtime.win-x86.Microsoft.NETCore.App", ""),
                ("runtime.win-x86.Microsoft.NETCore.App", "2.0.0"),
                ("runtime.win.System.Diagnostics.Debug", ""),
                ("runtime.win.System.Runtime.Extensions", ""),
                ("runtime.win-x86.Microsoft.NETCore.DotNetHostResolver", ""),
                ("runtime.win-x86.Microsoft.NETCore.DotNetAppHost", ""),
                ("runtime.win-x86.Microsoft.NETCore.DotNetAppHost", "2.0.0"),
                ("runtime.win-x86.Microsoft.NETCore.DotNetHostPolicy", ""),
            };

            foreach (var package in implicitDepenencies)
            {
                Console.WriteLine($"[{package.Item1}]");

                List<string> filepaths;

                if (!string.IsNullOrEmpty(package.Item2))
                {
                    var filepath = await new PackageDownloader().DownloadIfNeededAsync(
                        new PackageDependency(package.Item1, package.Item2), outputPath);
                    filepaths = new List<string> {filepath};
                }
                else
                {
                    filepaths = await new PackageDownloader().DownloadAllVersionsIfNeededAsync(package.Item1,
                        outputPath);
                }

                foreach (var filepath in filepaths)
                {
                    await new PackageHarvester().RunAsync(filepath, outputPath);
                }
            }

            await new PackageDownloader().DownloadAllMicrosoft();

            Console.WriteLine("DONE");
        }
    }
}