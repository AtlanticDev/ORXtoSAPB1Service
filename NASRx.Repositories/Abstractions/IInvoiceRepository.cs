using NASRx.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NASRx.Repositories.Abstractions
{
    public interface IInvoiceRepository
    {
        Task<IEnumerable<Invoice>> GetPendingInvoices();
    }
}