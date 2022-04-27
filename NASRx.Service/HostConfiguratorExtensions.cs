using SimpleInjector;
using Topshelf.HostConfigurators;

namespace NASRx.Service
{
    public static class HostConfiguratorExtensions
    {
        public static HostConfigurator UseSimpleInjector(this HostConfigurator configurator, Container container)
        {
            configurator.AddConfigurator(new SimpleInjectorHostBuilderConfigurator(container));
            return configurator;
        }
    }
}