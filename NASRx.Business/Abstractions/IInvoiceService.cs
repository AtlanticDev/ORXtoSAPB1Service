using NASRx.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NASRx.Business.Abstractions
{
    public interface IInvoiceService : IGenericService<Invoice, int>
    {
        Task<IEnumerable<Invoice>> GetPendingInvoices();

        Task<bool> UpdateAsNoPending(Invoice invoice);
    }
}