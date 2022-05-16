using NASRx.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NASRx.Repositories.Abstractions
{
    public interface IInvoiceRepository : IGenericRepository<Invoice, int>
    {
        Task<IEnumerable<Invoice>> GetPendingInvoices();

        Task<bool> UpdateAsNoPending(Invoice invoice);
    }
}