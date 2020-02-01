using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TruGet
{
    public class DependencyIdentifier
    {
        public async Task Run(string directoryPath)
        {
            foreach (var file in Directory.EnumerateFiles(directoryPath))
            {
                if (file.EndsWith(".csproj") || file.EndsWith(".props"))
                {
                    var lines = await File.ReadAllLinesAsync(file);

                    var packageReferences = lines
                        .Where(l => l.Contains("<PackageReference"))
                        .Select(l =>
                        {
                            var includeMatches = Regex.Matches(l, "Include=\"([^\"]*)\"");
                            var name = includeMatches[0].Groups[1].ToString();

                            var versionMatches = Regex.Matches(l, "Version=\"([^\"]*)\"");
                            var version = versionMatches[0].Groups[1].ToString();

                            return (name, version);
                        });

                    foreach (var pr in packageReferences)
                        Console.WriteLine(pr.name + "/" + pr.version);
                }
                else if (file.EndsWith("packages.config"))
                {
                }
            }

            foreach (var directory in Directory.EnumerateDirectories(directoryPath))
            {
                await Run(directory);
            }
        }
    }
}