using Autofac;
using Autofac.Features.ResolveAnything;
using AzureRepositories;
using AzureRepositories.Log;
using AzureStorage.Tables;
using Common;
using Common.Log;
using Core.Settings;
using LkeServices;

namespace QuantaApi
{
    public class AzureBinder
    {
        public const string DefaultConnectionString = "UseDevelopmentStorage=true";

        public ContainerBuilder Bind(BaseSettings settings)
        {
            var logToTable = new LogToTable(new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, "LogQuantaApiError", null),
                                            new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, "LogQuantaApiWarning", null),
                                            new AzureTableStorage<LogEntity>(settings.Db.LogsConnString, "LogQuantaApiInfo", null));
            var log = new LogToTableAndConsole(logToTable, new LogToConsole());
            var ioc = new ContainerBuilder();
            InitContainer(ioc, settings, log);
            return ioc;
        }

        private void InitContainer(ContainerBuilder ioc, BaseSettings settings, ILog log)
        {
#if DEBUG
            log.WriteInfoAsync("Quanta Api", "App start", null, $"BaseSettings : {settings.ToJson()}").Wait();
#else
            log.WriteInfoAsync("Quanta Api", "App start", null, $"BaseSettings : private").Wait();
#endif

            ioc.RegisterInstance(log);
            ioc.RegisterInstance(settings);
            
            ioc.BindAzure(settings, log);
            ioc.BindCommonServices();            

            ioc.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        }
    }
}
