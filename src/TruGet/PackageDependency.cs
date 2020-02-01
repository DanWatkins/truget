namespace TruGet
{
    public class PackageDependency
    {
        public PackageDependency(string id, string version)
        {
            Id = id;
            Version = version;
        }

        public string Id { get; }
        
        public string Version { get; }
    }
}