using NASRx.Model;

namespace NASRx.ModelConfig
{
    internal class InvoiceMapping : BaseMapping<Invoice, int>
    {
        internal InvoiceMapping() : base(tableName: "CustomerInvoiceHistory", idColumnName: "OID")
        {
            Map(t => t.BillToAddress1);
            Map(t => t.BillToAddress2);
            Map(t => t.BillToAddress3);
            Map(t => t.BillToCity);
            Map(t => t.BillToName);
            Map(t => t.BillToState);
            Map(t => t.BillToZip);
            Map(t => t.Comment);
            Map(t => t.CommissionPaidDate);
            Map(t => t.CompanyDivision);
            Map(t => t.CustomerId);
            Map(t => t.CustomerName);
            Map(t => t.CustomerPoNumber);
            Map(t => t.DeliveryEmail);
            Map(t => t.DiscountDueDate);
            Map(t => t.FrieghtAmount);
            Map(t => t.GCRecord);
            Map(t => t.InvoiceDate);
            Map(t => t.InvoiceDueDate);
            Map(t => t.InvoiceNumber);
            Map(t => t.InvoicePDF);
            Map(t => t.InvoicePdfFile);
            Map(t => t.InvoiceTermCode);
            Map(t => t.InvoiceTotal);
            Map(t => t.InvoiceType);
            Map(t => t.IsPrinted);
            Map(t => t.LastDeliveryDate);
            Map(t => t.OldInvoiceNumber);
            Map(t => t.OptimisticLockField);
            Map(t => t.SalesOrderNumber);
            Map(t => t.SalesRep);
            Map(t => t.SapDocumentEntry);
            Map(t => t.SapEntered);
            Map(t => t.ShipDate);
            Map(t => t.ShipToAddress1);
            Map(t => t.ShipToAddress2);
            Map(t => t.ShipToAddress3);
            Map(t => t.ShipToCity);
            Map(t => t.ShipToState);
            Map(t => t.ShipToZip);
            Map(t => t.TotalCommissionEarned);
            Map(t => t.TrackingNumber);
            Map(t => t.TransactionDate);
            Map(t => t.WhseCode);
        }
    }
}