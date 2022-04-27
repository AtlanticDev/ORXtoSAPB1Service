using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NASRx.IoC;
using NASRx.ModelConfig;
using NASRx.Utilities;
using System;
using Topshelf;

namespace NASRx.Service
{
    internal class Program
    {
        private const string APP_SETTINGS = "appsettings.json";

        internal static void Main()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile(APP_SETTINGS).Build();
            var container = IoCContainer.Create();

            container.Register<Processor>();

            NASRxSettings.Initialize(configuration);
            ModelMapper.InitializeMapping();

            var code = HostFactory.Run(hc =>
            {
                hc.UseSimpleInjector(container);

                hc.Service<Processor>(sc =>
                {
                    sc.ConstructUsingSimpleInjector();
                    sc.WhenStarted(p => p.Start());
                    sc.WhenStopped(p => p.Stop());
                });

                hc.RunAsLocalSystem();
                hc.SetDisplayName("ORX to SAP Service");
                hc.SetServiceName("ORX2SAP.Service");
                hc.SetDescription("ORX to SAP Synch Service");
            });

            Environment.ExitCode = (int)Convert.ChangeType(code, code.GetTypeCode());
        }
    }
}