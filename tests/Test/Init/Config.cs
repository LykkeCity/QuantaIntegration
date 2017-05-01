using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using AzureRepositories;
using Common.Log;
using Core.Settings;
using LkeServices;
using NUnit.Framework;
using Microsoft.Extensions.DependencyInjection;

// ReSharper disable once CheckNamespace
namespace Test
{
    [SetUpFixture]
    public class Config
    {
        public static IServiceProvider Services { get; set; }
        public static ILog Logger => Services.GetService<ILog>();

        [OneTimeSetUp]
        public void Initialize()
        {
            var dic = Directory.GetCurrentDirectory();
            var settings = GeneralSettingsReader.ReadGeneralSettingsLocal<BaseSettings>(Path.Combine(dic, "../../../../settings/quantasettings.json"));

            var log = new LogToConsole();

            ContainerBuilder builder = new ContainerBuilder();
            builder.RegisterInstance(settings);
            builder.RegisterInstance(log).As<ILog>();
            builder.BindAzure(settings, log);
            builder.BindCommonServices();

            Services = new AutofacServiceProvider(builder.Build());
        }
    }
}
