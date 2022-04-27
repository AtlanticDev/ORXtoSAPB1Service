using Topshelf.ServiceConfigurators;

namespace NASRx.Service
{
    public static class ServiceConfiguratorExtensions
    {
        public static ServiceConfigurator<T> ConstructUsingSimpleInjector<T>(this ServiceConfigurator<T> configurator) where T : class
        {
            configurator.ConstructUsing(serviceFactory => SimpleInjectorHostBuilderConfigurator.Container.GetInstance<T>());
            return configurator;
        }
    }
}