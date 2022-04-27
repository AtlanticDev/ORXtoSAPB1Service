using NASRx.Model;

namespace NASRx.ModelConfig
{
    internal class InvoiceDetailMapping : BaseMapping<InvoiceDetail, int>
    {
        internal InvoiceDetailMapping() : base("CustomerInvoiceHistoryDetails", "OID")
        {
            Map(d => d.CommissionCalculatedDateTime);
            Map(d => d.ExtendedAmount).ToColumn("ExtendedAmt");
            Map(d => d.GCRecord);
            Map(d => d.GICogSoldAcct);
            Map(d => d.GISalesAcct);
            Map(d => d.InvoiceNumber);
            Map(d => d.ItemCommissionEarned);
            Map(d => d.ItemCommissionPct);
            Map(d => d.ItemDescription);
            Map(d => d.ItemLot);
            Map(d => d.ItemNumber);
            Map(d => d.LotExpDate);
            Map(d => d.OptimisticLockField);
            Map(d => d.ProductLine);
            Map(d => d.QtyBackOrdered);
            Map(d => d.QtyOrdered);
            Map(d => d.QtyShipped);
            Map(d => d.SortOrder);
            Map(d => d.UnitCost);
            Map(d => d.UnitOfMeasure).ToColumn("UnitOfMessure");
            Map(d => d.UnitPrice);
            Map(d => d.WarehouseId);
        }
    }
}