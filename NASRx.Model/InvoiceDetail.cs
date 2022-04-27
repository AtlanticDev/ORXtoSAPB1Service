using System;

namespace NASRx.Model
{
    public class InvoiceDetail : Entity<int>
    {
        public DateTime? CommissionCalculatedDateTime { get; set; }

        public decimal? ExtendedAmount { get; set; }

        public int? GCRecord { get; set; }

        public string GICogSoldAcct { get; set; }

        public string GISalesAcct { get; set; }

        public int? InvoiceNumber { get; set; }

        public double? ItemCommissionEarned { get; set; }

        public double? ItemCommissionPct { get; set; }

        public string ItemDescription { get; set; }

        public string ItemLot { get; set; }

        public string ItemNumber { get; set; }

        public DateTime? LotExpDate { get; set; }

        public int? OptimisticLockField { get; set; }

        public int? ProductLine { get; set; }

        public decimal? QtyBackOrdered { get; set; }

        public decimal? QtyOrdered { get; set; }

        public decimal? QtyShipped { get; set; }

        public int? SortOrder { get; set; }

        public decimal? UnitCost { get; set; }

        public string UnitOfMeasure { get; set; }

        public decimal? UnitPrice { get; set; }

        public string WarehouseId { get; set; }
    }
}