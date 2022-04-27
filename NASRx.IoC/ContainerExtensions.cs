using NASRx.Business.Abstractions;
using NASRx.Business.Concretes;
using NASRx.Infra.Abstractions;
using NASRx.Infra.Concretes;
using NASRx.Repositories.Abstractions;
using NASRx.Repositories.Concretes;
using SimpleInjector;

namespace NASRx.IoC
{
    public static class ContainerExtensions
    {
        public static void RegisterInfra(this Container container)
        {
            container.Register<IContext, Context>();
            container.Register<ILogging, Logging>();
            container.Register<IUnitOfWork, UnitOfWork>();
        }

        public static void RegisterRepository(this Container container)
        {
            container.Register<IInvoiceRepository, InvoiceRepository>();
        }

        public static void RegisterService(this Container container)
        {
            container.Register<ISAPB1Service, SAPB1Service>();
            container.Register<IInvoiceService, InvoiceService>();
        }
    }
}