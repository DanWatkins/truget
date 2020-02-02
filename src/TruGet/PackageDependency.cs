using NuGet.Versioning;

namespace TruGet
{
    public class PackageDependency
    {
        public PackageDependency(string id, string version)
        {
            Id = id;
            Version = version;
        }
        
        public PackageDependency(string id, VersionRange versionRange)
        {
            Id = id;

            if (versionRange.HasUpperBound)
                Version = versionRange.MaxVersion.ToString();
            else if (versionRange.HasLowerBound)
                Version = versionRange.MinVersion.ToString();
            else
                Version = versionRange.ToShortString();
        }

        public PackageDependency(string id, NuGetVersion version)
        {
            Id = id;
            Version = version.ToNormalizedString();
        }

        public string Id { get; }
        
        public string Version { get; }
    }
}