using NASRx.Model.Annotation;
using System;
using System.Collections.Generic;

namespace NASRx.Model
{
    public class Invoice : Entity<int>
    {
        public string BillToAddress1 { get; set; }

        public string BillToAddress2 { get; set; }

        public string BillToAddress3 { get; set; }

        public string BillToCity { get; set; }

        public string BillToName { get; set; }

        public string BillToState { get; set; }

        public string BillToZip { get; set; }

        public string Comment { get; set; }

        public DateTime? CommissionPaidDate { get; set; }

        public string CompanyDivision { get; set; }

        public string CustomerId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerNo { get; set; }

        public string CustomerPoNumber { get; set; }

        public string DeliveryEmail { get; set; }

        public DateTime? DiscountDueDate { get; set; }

        public decimal? FrieghtAmount { get; set; }

        public int? GCRecord { get; set; }

        public DateTime? InvoiceDate { get; set; }

        public DateTime? InvoiceDueDate { get; set; }

        public string InvoicePDF { get; set; }

        public Guid? InvoicePdfFile { get; set; }

        public string InvoiceNumber { get; set; }

        public string InvoiceTermCode { get; set; }

        public decimal? InvoiceTotal { get; set; }

        public int? InvoiceType { get; set; }

        public bool? IsPrinted { get; set; }

        [IgnoreOnInsert]
        [IgnoreOnUpdate]
        public IEnumerable<InvoiceDetail> Items { get; set; } = new List<InvoiceDetail>();

        public DateTime? LastDeliveryDate { get; set; }

        public string OldInvoiceNumber { get; set; }

        public int? OptimisticLockField { get; set; }

        public string SalesOrderNumber { get; set; }

        public string SalesRep { get; set; }

        public long? SapDocumentEntry { get; set; }

        public DateTime? SapEntered { get; set; }

        public DateTime? ShipDate { get; set; }

        public string ShipToAddress1 { get; set; }

        public string ShipToAddress2 { get; set; }

        public string ShipToAddress3 { get; set; }

        public string ShipToCity { get; set; }

        public string ShipToState { get; set; }

        public string ShipToZip { get; set; }

        public decimal? TotalCommissionEarned { get; set; }

        public string TrackingNumber { get; set; }

        public DateTime? TransactionDate { get; set; }

        public string WhseCode { get; set; }
    }
}