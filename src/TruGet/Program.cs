using System;
using System.Threading.Tasks;

namespace TruGet
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            const string repositoryPath = @"C:\Users\dwatk\code\realityfocus-forks\Avalonia";

            await new DependencyIdentifier().Run(repositoryPath);
            
            Console.WriteLine("DONE");
        }
    }
}
