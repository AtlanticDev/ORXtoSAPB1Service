using Dapper.FluentMap;

namespace NASRx.ModelConfig
{
    public class ModelMapper
    {
        public static void InitializeMapping()
        {
            FluentMapper.Initialize(config =>
            {
                config.AddMap(new InvoiceMapping());
                config.AddMap(new InvoiceDetailMapping());
            });
        }
    }
}