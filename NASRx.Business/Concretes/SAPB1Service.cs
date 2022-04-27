using NASRx.Business.Abstractions;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Utilities;
using SAPbobsCOM;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace NASRx.Business.Concretes
{
    public class SAPB1Service : ISAPB1Service
    {
        private readonly ICompany _company;
        private readonly ILogging _logging;

        public SAPB1Service(ILogging logging)
        {
            _company = new Company
            {
                CompanyDB = NASRxSettings.Instance.SAPSettings.CompanyDb,
                DbServerType = BoDataServerTypes.dst_MSSQL2016,
                Password = NASRxSettings.Instance.SAPSettings.Password,
                Server = NASRxSettings.Instance.SAPSettings.Server,
                UserName = NASRxSettings.Instance.SAPSettings.UserName,
                UseTrusted = false
            };
            _logging = logging;
        }

        public bool CheckIfBusinessPartnerExist(string bpKey)
        {
            if (string.IsNullOrWhiteSpace(bpKey))
                return false;

            try
            {
                var partner = (BusinessPartners)_company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
                var result = partner.GetByKey(bpKey.ToUpper());

                Marshal.ReleaseComObject(partner);

                return result;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex);
                return false;
            }
        }

        public void CreateBusinessPartner(Invoice transaction)
        {
            if (string.IsNullOrWhiteSpace(transaction.CustomerId))
                return;

            var partner = (BusinessPartners)_company.GetBusinessObject(BoObjectTypes.oBusinessPartners);
            var addresses = partner.Addresses;

            partner.CardCode = transaction.CustomerId;
            partner.CardName = transaction.CustomerName;
            partner.CardType = BoCardTypes.cCustomer;
            partner.EmailAddress = transaction.DeliveryEmail;

            addresses.AddressName = transaction.BillToAddress1;
            addresses.AddressName2 = transaction.BillToAddress2;
            addresses.AddressName3 = transaction.BillToAddress3;
            addresses.AddressType = BoAddressType.bo_BillTo;
            addresses.City = transaction.BillToCity;
            addresses.State = transaction.BillToState;
            addresses.ZipCode = transaction.BillToZip;
            addresses.Add();

            addresses.AddressName = transaction.ShipToAddress1;
            addresses.AddressName2 = transaction.ShipToAddress2;
            addresses.AddressName3 = transaction.ShipToAddress3;
            addresses.AddressType = BoAddressType.bo_ShipTo;
            addresses.City = transaction.ShipToCity;
            addresses.State = transaction.ShipToState;
            addresses.ZipCode = transaction.ShipToZip;
            addresses.Add();

            partner.Add();
        }

        public void CreateInvoice(Invoice transaction)
        {
            var invoice = (Documents)_company.GetBusinessObject(BoObjectTypes.oInvoices);
            var invoiceItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);

            invoice.CardCode = transaction.TrackingNumber;
            invoice.DocType = BoDocumentTypes.dDocument_Service;
            invoice.DocTotal = Convert.ToDouble(transaction.Items.Sum(i => i.QtyShipped.GetValueOrDefault(0M) * i.UnitPrice.GetValueOrDefault(0M)));
            invoice.HandWritten = BoYesNoEnum.tNO;

            if (transaction.InvoiceDate.HasValue)
                invoice.DocDate = transaction.InvoiceDate.Value;

            if (transaction.InvoiceDueDate.HasValue)
                invoice.DocDueDate = transaction.InvoiceDueDate.Value;

            invoice.Add();

            foreach (var item in transaction.Items)
            {
                invoiceItem.ItemCode = item.ItemNumber;
                invoiceItem.ItemName = item.ItemDescription;
                invoiceItem.ItemType = ItemTypeEnum.itItems;
                // invoiceItem.ItemClass = ItemClassEnum.itcService;
                invoiceItem.InventoryItem = BoYesNoEnum.tNO;

                invoiceItem.SalesUnit = item.QtyShipped.ToString();
                invoiceItem.AvgStdPrice = Convert.ToDouble(item.UnitPrice.GetValueOrDefault(0M));
                invoiceItem.ProdStdCost = Convert.ToDouble(item.UnitCost.GetValueOrDefault(0M));
                // invoiceItem.????? = item.QtyShipped.GetValueOrDefault(0M) * item.UnitPrice.GetValueOrDeafult(0M);
                invoiceItem.WhsInfo.WarehouseCode = item.WarehouseId;
                invoiceItem.Add();
            }
        }

        public (bool result, string errorMsg) Connect()
        {
            try
            {
                var result = _company.Connect();
                if (!_company.Connected)
                {
                    var buffer = new StringBuilder();
                    buffer.AppendLine("Not connected to SAP");
                    buffer.AppendLine($"License Server: {_company.LicenseServer}");
                    buffer.AppendLine($"SLD: {_company.SLDServer}");
                    buffer.AppendLine($"User: {_company.UserName}");
                    buffer.AppendLine($"Error: {_company.GetLastErrorCode()} - {_company.GetLastErrorDescription()}");
                    return (false, buffer.ToString());
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                _logging.LogError(ex);
                return (false, ex.Message);
            }
        }

        public bool Disconnect()
        {
            try
            {
                _company.Disconnect();
                return true;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex);
                return false;
            }
        }
    }
}