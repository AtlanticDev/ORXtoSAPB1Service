using Microsoft.Extensions.Configuration;

namespace NASRx.Utilities
{
    public class NASRxSettings
    {
        private NASRxSettings() { }

        public static NASRxSettings Instance { get; private set; }

        public string ConnectionString { get; internal set; }

        public string EventSource { get; set; }

        public SAPSettings SAPSettings { get; set; }

        public static void Initialize(IConfiguration configuration)
        {
            Instance = new();

            Instance.ConnectionString = configuration.GetConnectionString("DefaultConnection");
            var section = configuration.GetSection(nameof(NASRxSettings));
            section.Bind(Instance, options => options.BindNonPublicProperties = true);
        }
    }
}