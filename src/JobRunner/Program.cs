using System;
using QuantaJob;

namespace JobRunner
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Clear();
            Console.Title = "Ethereum job - Ver. " + Microsoft.Extensions.PlatformAbstractions.PlatformServices.Default.Application.ApplicationVersion;

            var host = new AppHost();

            Console.WriteLine($"Ethereum job is running");
            Console.WriteLine("Utc time: " + DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"));

            host.Run();
        }
    }
}
