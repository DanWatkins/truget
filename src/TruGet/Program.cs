using System;
using System.Threading.Tasks;

namespace TruGet
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            const string path = @"C:\Users\dwatk\Desktop\avalonia.0.9.2.nupkg";
            const string output = @"C:\Users\dwatk\Desktop\avalonia.0.9.2.nupkg.dependencies\";
            
            await PackageHarvester.RunAsync(path, output);
            Console.WriteLine("DONE");
        }
    }
}
