using NASRx.Business.Abstractions;
using NASRx.Infra.Abstractions;
using NASRx.Model;
using NASRx.Repositories.Abstractions;
using NASRx.Utilities;
using SAPbobsCOM;
using System;
using System.Runtime.InteropServices;
using System.Text;

namespace NASRx.Business.Concretes
{
    public class SAPB1Service : ISAPB1Service
    {
        private readonly ICompany _company;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly ILogging _logging;

        public SAPB1Service(IInvoiceRepository invoiceRepository, ILogging logging)
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
            _invoiceRepository = invoiceRepository;
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

        public void CreateBusinessPartner(Invoice invoice)
        {
            if (string.IsNullOrWhiteSpace(invoice.CustomerNo))
                return;

            try
            {
                var partner = (BusinessPartners)_company.GetBusinessObject(BoObjectTypes.oBusinessPartners);

                partner.CardCode = invoice.CustomerNo;
                partner.CardName = invoice.CustomerName;
                partner.CardType = BoCardTypes.cCustomer;
                partner.EmailAddress = invoice.DeliveryEmail;

                // added this because getting error
                partner.Address = invoice.ShipToAddress1;
                partner.City = invoice.ShipToCity;
                partner.BillToState = invoice.ShipToState;
                partner.ZipCode = invoice.ShipToZip;
                partner.MailAddress = invoice.ShipToAddress1;
                partner.MailCity = invoice.ShipToCity;
                partner.MailZipCode = invoice.ShipToZip;

                var result = partner.Add();
                if (result != 0)
                    _logging.LogDebug($"Failed to add customer {partner.CardCode}: {_company.GetLastErrorDescription()}");

                var addresses = partner.Addresses;

                addresses.SetCurrentLine(0);
                addresses.AddressName = invoice.BillToAddress1;
                addresses.AddressName2 = invoice.BillToAddress2;
                addresses.AddressName3 = invoice.BillToAddress3;
                addresses.AddressType = BoAddressType.bo_BillTo;
                addresses.City = invoice.BillToCity;
                addresses.State = invoice.BillToState;
                addresses.ZipCode = invoice.BillToZip;
                addresses.Add();

                addresses.SetCurrentLine(1);
                addresses.AddressName = invoice.ShipToAddress1;
                addresses.AddressName2 = invoice.ShipToAddress2;
                addresses.AddressName3 = invoice.ShipToAddress3;
                addresses.AddressType = BoAddressType.bo_ShipTo;
                addresses.City = invoice.ShipToCity;
                addresses.State = invoice.ShipToState;
                addresses.ZipCode = invoice.ShipToZip;
                addresses.Add();
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.Message);
                throw;
            }
        }

        public int CreateNonInventoryItem(InvoiceDetail item)
        {
            _logging.LogDebug($"Creating item {item.ItemNumber}");

            var sapItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);

            sapItem.ItemCode = item.ItemNumber;
            sapItem.ItemName = item.ItemDescription;
            sapItem.ItemType = ItemTypeEnum.itItems;
            sapItem.InventoryItem = BoYesNoEnum.tNO;
            sapItem.SalesItem = BoYesNoEnum.tYES;
            sapItem.PurchaseItem = BoYesNoEnum.tNO;
            sapItem.VatLiable = BoYesNoEnum.tNO;
            sapItem.ArTaxCode = "EX";
            sapItem.SalesUnit = "EACH";
            sapItem.InventoryUOM = "EACH";
            sapItem.GLMethod = BoGLMethods.glm_ItemClass;
            sapItem.TaxType = BoTaxTypes.tt_No;

            var result = sapItem.Add();
            if (result != 0)
            {
                _logging.LogDebug($"Unable to add item {item.ItemNumber}: {_company.GetLastErrorCode()} - {_company.GetLastErrorDescription()}");
                return -1;
            }
            return result;
        }

        public void CreateInvoice(Invoice invoice)
        {
            var sapInvoice = (Documents)_company.GetBusinessObject(BoObjectTypes.oInvoices);

            sapInvoice.CardCode = invoice.CustomerNo;
            sapInvoice.DocType = BoDocumentTypes.dDocument_Items;
            sapInvoice.NumAtCard = invoice.CustomerPoNumber;
            sapInvoice.HandWritten = BoYesNoEnum.tNO;
            sapInvoice.DocNum = Convert.ToInt32(invoice.InvoiceNumber);

            if (invoice.InvoiceDate.HasValue)
            {
                sapInvoice.DocDate = invoice.InvoiceDate.Value;
                sapInvoice.TaxDate = invoice.InvoiceDate.Value;
            }

            if (invoice.InvoiceDueDate.HasValue)
                sapInvoice.DocDueDate = invoice.InvoiceDueDate.Value;

            var lineNo = 0;
            foreach (var item in invoice.Items)
            {
                var sapItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);
                if (!sapItem.GetByKey(item.ItemNumber.ToString()))
                    CreateNonInventoryItem(item);

                var lines = sapInvoice.Lines;
                lines.SetCurrentLine(lineNo);

                lines.ItemCode = item.ItemNumber;
                lines.ItemDescription = item.ItemDescription;
                lines.Quantity = Convert.ToDouble(item.QtyShipped);
                lines.UnitPrice = Convert.ToDouble(item.UnitPrice);
                lines.DiscountPercent = 0D;
                lines.TaxType = BoTaxTypes.tt_No;
                lines.LineType = BoDocLineType.dlt_Regular;
                lines.Add();
            }

            if (sapInvoice.Add() != 0)
                _logging.LogDebug($"Failed to add customer's invoice {sapInvoice.CardCode}: {_company.GetLastErrorCode()} - {_company.GetLastErrorDescription()}");
            else
            {
                invoice.SapEntered = DateTime.Now;
                invoice.SapDocumentEntry = sapInvoice.DocNum;

                _invoiceRepository.UpdateAsNoPending(invoice);
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