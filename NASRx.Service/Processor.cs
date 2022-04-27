using NASRx.Business.Abstractions;
using NASRx.Utilities;
using System.Threading;
using System.Threading.Tasks;

namespace NASRx.Service
{
    internal class Processor
    {
        private const int INITIAL_POLLING = 60000;
        private const int MAX_POLLING = 18000000;

        private CancellationTokenSource _tokenSource;
        private readonly IInvoiceService _invoiceService;
        private readonly ISAPB1Service _sapB1Service;

        public Processor(IInvoiceService invoiceService, ISAPB1Service sapB1Service)
        {
            _invoiceService = invoiceService;
            _sapB1Service = sapB1Service;
        }

        private static int GetPollingInterval(int multiplier)
        {
            var result = INITIAL_POLLING * multiplier;
            return result > MAX_POLLING ? result : MAX_POLLING;
        }

        private async Task Process()
        {
            var currentMultiplier = 1;
            while (true)
            {
                var invoices = await _invoiceService.GetPendingInvoices();
                if (invoices.IsNullOrEmpty())
                    currentMultiplier *= 2;
                else
                {
                    
                    foreach (var invoice in invoices)
                    {

                        if (!_sapB1Service.CheckIfBusinessPartnerExist(invoice.CustomerId))
                        {
                            Logging.LogDebug($"Adding customer {invoice.CustomerId}");
                            _sapB1Service.CreateBusinessPartner(invoice);
                        }
                        _sapB1Service.CreateInvoice(invoice);
                        Logging.LogDebug(invoice.InvoiceNumber);
                        if (_tokenSource.IsCancellationRequested)
                            break;
                    }
                    currentMultiplier = 1;
                }

                if (_tokenSource.IsCancellationRequested)
                    break;
                await Task.Delay(GetPollingInterval(currentMultiplier));
            }
        }

        public void Start()
        {
            var (result, errorMsg) = _sapB1Service.Connect();
            if (!result)
            {
                Logging.LogDebug(errorMsg);
                return;
            }
            _tokenSource = new CancellationTokenSource();
            Task.Factory.StartNew(async () => await Process(), _tokenSource.Token);
        }

        public void Stop()
        {
            _sapB1Service.Disconnect();
            _tokenSource.Cancel();
        }
    }
}