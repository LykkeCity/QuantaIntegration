using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Features.ResolveAnything;
using AzureStorage.Tables;
using Common.Log;
using Core.Settings;
using AzureRepositories;
using AzureRepositories.Log;
using Common;
using LkeServices;
using Lykke.JobTriggers.Extenstions;
using Microsoft.Extensions.DependencyInjection;

namespace QuantaJob
{
    public class AzureBinder
    {
        public const string DefaultConnectionString = "UseDevelopmentStorage=true";

        public ContainerBuilder Bind(BaseSettings settings)
        {
            var logToTable = new LogToTable(new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, "LogQuantaJobError", null),
                                            new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, "LogQuantaJobWarning", null),
                                            new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, "LogQuantaJobInfo", null));
            var log = new LogToTableAndConsole(logToTable, new LogToConsole());
            var ioc = new ContainerBuilder();
            InitContainer(ioc, settings, log);
            return ioc;
        }

        private void InitContainer(ContainerBuilder ioc, BaseSettings settings, ILog log)
        {
#if DEBUG
            log.WriteInfoAsync("Quanta Job", "App start", null, $"BaseSettings : {settings.ToJson()}").Wait();
#else
            log.WriteInfoAsync("Quanta Job", "App start", null, $"BaseSettings : private").Wait();
#endif
            ioc.RegisterInstance(log);
            ioc.RegisterInstance(settings);

            ioc.BindCommonServices();
            ioc.BindAzure(settings, log);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton(log);

            serviceCollection.AddTriggers(pool =>
            {
                pool.AddDefaultConnection(settings.Db.DataConnString);
                pool.AddConnection("cashout", settings.Db.QuantaSrvConnString);
            });
            ioc.Populate(serviceCollection);

            ioc.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }
    }
}
