using NASRx.Business.Abstractions;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NASRx.Business.Concretes
{
    public class InvoiceService : IInvoiceService
    {
        private readonly ILogging _logging;
        private readonly IInvoiceRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public InvoiceService(ILogging logging, IInvoiceRepository repository, IUnitOfWork unitOfWork)
        {
            _logging = logging;
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Invoice>> GetPendingInvoices()
        {
            try
            {
                return await _repository.GetPendingInvoices();
            }
            catch (Exception ex)
            {
                _logging.LogError(ex);
                return null;
            }
        }
    }
}