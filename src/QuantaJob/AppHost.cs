using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading;
using Autofac.Extensions.DependencyInjection;
using AzureRepositories;
using Core.Settings;
using Lykke.JobTriggers.Triggers;
using Microsoft.Extensions.Configuration;

namespace QuantaJob
{
    public class AppHost
    {
        public IConfigurationRoot Configuration { get; }

        public AppHost()
        {
            var builder = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory())
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public void Run()
        {
            var settings = GeneralSettingsReader.ReadGeneralSettings<GeneralSettings>(Configuration.GetConnectionString("Settings"));

            var containerBuilder = new AzureBinder().Bind(settings.QuantaJobs);
            var ioc = containerBuilder.Build();

            var triggerHost = new TriggerHost(new AutofacServiceProvider(ioc));

            triggerHost.ProvideAssembly(GetType().GetTypeInfo().Assembly);

            var end = new ManualResetEvent(false);

            AssemblyLoadContext.Default.Unloading += ctx =>
            {
                Console.WriteLine("SIGTERM recieved");
                triggerHost.Cancel();

                end.WaitOne();
            };

            triggerHost.StartAndBlock();
            end.Set();
        }
    }
}
