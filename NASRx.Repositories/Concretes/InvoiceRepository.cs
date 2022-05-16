using Dapper;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Repositories.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace NASRx.Repositories.Concretes
{
    public class InvoiceRepository : GenericRepository<Invoice, int>, IInvoiceRepository
    {
        public InvoiceRepository(IContext context) : base(context) { }

        public async Task<IEnumerable<Invoice>> GetPendingInvoices()
        {
            var sql = "SELECT C.CustomerNo, H.* FROM CustomerInvoiceHistory H, Customer C WHERE C.OID = H.CustomerID AND H.SapEntered IS NULL AND H.InvoiceDate IS NOT NULL ORDER BY H.InvoiceDate, H.CustomerId ";
            sql += "SELECT CIHD.* FROM CustomerInvoiceHistory CIH INNER JOIN CustomerInvoiceHistoryDetails CIHD ON CIH.OID = CIHD.InvoiceNumber WHERE CIH.SapEntered IS NULL";

            var multi = await Context.Connection.QueryMultipleAsync(transaction: Context.Transaction, commandType: CommandType.Text, sql: sql);
            var invoices = multi.Read<Invoice>();
            var items = multi.Read<InvoiceDetail>();

            foreach (var invoice in invoices)
            {
                invoice.Items = items.Where(i => i.InvoiceNumber == invoice.Id).OrderBy(i => i.SortOrder);
            }
            return invoices;
        }

        public async Task<bool> UpdateAsNoPending(Invoice invoice)
        {
            if (invoice == null)
                throw new ArgumentNullException(nameof(invoice));

            if (invoice.Id <= 0)
                throw new ArgumentException($"Invalid {nameof(Invoice)}.{nameof(invoice.Id)} value");

            if (!invoice.SapDocumentEntry.HasValue)
                throw new ArgumentException($"Invalid {nameof(Invoice)}.{nameof(invoice.SapDocumentEntry)} value");

            if (!invoice.SapEntered.HasValue)
                throw new ArgumentException($"Invalid {nameof(Invoice)}.{nameof(invoice.SapEntered)} value");

            var sql = "UPDATE CustomerInvoiceHistory SET SapEntered = @SapEntered AND SapDocumentEntry = @SapDocumentEntry WHERE OID = @Id";
            var parameters = new DynamicParameters();

            parameters.Add($"@{nameof(invoice.Id)}", invoice.Id);
            parameters.Add($"@{nameof(invoice.SapDocumentEntry)}", invoice.SapDocumentEntry);
            parameters.Add($"@{nameof(invoice.SapEntered)}", invoice.SapEntered);

            return (await Context.Connection.ExecuteAsync(transaction: Context.Transaction, commandType: CommandType.Text, sql: sql, param: parameters)) == 1;
        }
    }
}