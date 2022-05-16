using NASRx.Business.Abstractions;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NASRx.Business.Concretes
{
    public class InvoiceService : GenericService<Invoice, int>, IInvoiceService
    {
        public InvoiceService(ILogging logging, IInvoiceRepository repository, IUnitOfWork unitOfWork)
            : base(logging, repository, unitOfWork) { }

        public async Task<IEnumerable<Invoice>> GetPendingInvoices()
        {
            try
            {
                return await (Repository as IInvoiceRepository).GetPendingInvoices();
            }
            catch (Exception ex)
            {
                Logging.LogError(ex);
                return null;
            }
        }

        public async Task<bool> UpdateAsNoPending(Invoice invoice)
        {
            try
            {
                UnitOfWork.BeginTransaction();
                var result = await (Repository as IInvoiceRepository).UpdateAsNoPending(invoice);
                UnitOfWork.CommitTransaction();
                return result;
            }
            catch (Exception ex)
            {
                UnitOfWork.RollbackTransaction();
                Logging.LogError(ex);
                return false;
            }
        }
    }
}