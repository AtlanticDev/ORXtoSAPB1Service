using Dapper;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Repositories.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NASRx.Repositories.Concretes
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IContext _context;

        public InvoiceRepository(IContext context)
            => _context = context;

        public async Task<IEnumerable<Invoice>> GetPendingInvoices()
        {
            var sql = "SELECT * FROM CustomerInvoiceHistory WHERE SapEntered IS NULL AND InvoiceDate IS NOT NULL ORDER BY OID ";
            sql += "SELECT CIHD.* FROM CustomerInvoiceHistory CIH INNER JOIN CustomerInvoiceHistoryDetails CIHD ON CIH.OID = CIHD.InvoiceNumber WHERE CIH.SapEntered IS NULL";

            var multi = await _context.Connection.QueryMultipleAsync(transaction: _context.Transaction, commandType: CommandType.Text, sql: sql);
            var invoices = multi.Read<Invoice>();
            var items = multi.Read<InvoiceDetail>();

            foreach (var invoice in invoices)
            {
                invoice.Items = items.Where(i => i.InvoiceNumber == invoice.Id).OrderBy(i => i.SortOrder);
            }
            return invoices;
        }
    }
}