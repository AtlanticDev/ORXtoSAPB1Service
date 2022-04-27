using NASRx.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NASRx.Business.Abstractions
{
    public interface IInvoiceService
    {
        Task<IEnumerable<Invoice>> GetPendingInvoices();
    }
}