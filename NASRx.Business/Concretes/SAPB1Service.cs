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

            _company = new Company {
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
            try
            {

                var partner = (BusinessPartners)_company.GetBusinessObject(BoObjectTypes.oBusinessPartners);


                partner.CardCode = transaction.CustomerId;
                partner.CardName = transaction.CustomerName;
                partner.CardType = BoCardTypes.cCustomer;
                partner.EmailAddress = transaction.DeliveryEmail;

                // added this becuase getting error
                partner.Address = transaction.ShipToAddress1;
                partner.City = transaction.ShipToCity;
                partner.BillToState = transaction.ShipToState;
                partner.ZipCode = transaction.ShipToZip;
                partner.MailAddress = transaction.ShipToAddress1;
                partner.MailCity = transaction.ShipToCity;
                partner.MailZipCode = transaction.ShipToZip;
                int ret = partner.Add();
                if (ret != 0)
                {
                    string err = _company.GetLastErrorDescription();
                    Logging.LogDebug($"failed to add Customer {partner.CardCode} {err}");
                }


                var addresses = partner.Addresses;

                addresses.SetCurrentLine(0);
                addresses.AddressName = transaction.BillToAddress1;
                addresses.AddressName2 = transaction.BillToAddress2;
                addresses.AddressName3 = transaction.BillToAddress3;
                addresses.AddressType = BoAddressType.bo_BillTo;
                addresses.City = transaction.BillToCity;
                addresses.State = transaction.BillToState;
                addresses.ZipCode = transaction.BillToZip;
                addresses.Add();


                addresses.SetCurrentLine(1);
                addresses.AddressName = transaction.ShipToAddress1;
                addresses.AddressName2 = transaction.ShipToAddress2;
                addresses.AddressName3 = transaction.ShipToAddress3;
                addresses.AddressType = BoAddressType.bo_ShipTo;
                addresses.City = transaction.ShipToCity;
                addresses.State = transaction.ShipToState;
                addresses.ZipCode = transaction.ShipToZip;
                addresses.Add();


            }
            catch (Exception ex)
            {
                Logging.LogError(ex.Message);
                throw;
            }

        }

        public int CreateNonInventoryItem(InvoiceDetail item)
        {
            Logging.LogDebug($"Creating Item {item.ItemNumber}");
            Items vItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);
            vItem.ItemCode = item.ItemNumber;
            vItem.ItemName = item.ItemDescription;
            vItem.ItemType = ItemTypeEnum.itItems;
            vItem.InventoryItem = BoYesNoEnum.tNO;
            vItem.SalesItem = BoYesNoEnum.tYES;
            vItem.PurchaseItem = BoYesNoEnum.tNO;
            vItem.VatLiable = BoYesNoEnum.tNO;
            vItem.ArTaxCode = "EX";
            vItem.SalesUnit = "EACH";
            vItem.InventoryUOM = "EACH";
            vItem.GLMethod = BoGLMethods.glm_ItemClass;



            vItem.TaxType = BoTaxTypes.tt_No;
            int ret = vItem.Add();
            if (ret != 0)
            {
                string err = _company.GetLastErrorCode().ToString() + " - " + _company.GetLastErrorDescription();
                Logging.LogDebug($"Unable to add item { item.ItemNumber} {err}");
                return -1;
            }
            return 0;
        }



        public void CreateInvoice(Invoice transaction)
        {
            var invoice = (Documents)_company.GetBusinessObject(BoObjectTypes.oInvoices);
            var invoiceItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);

            invoice.CardCode = transaction.CustomerId;

            invoice.DocType = BoDocumentTypes.dDocument_Items;
            invoice.NumAtCard = transaction.CustomerPoNumber;
            // invoice.DocTotal = Convert.ToDouble(transaction.Items.Sum(i => i.QtyShipped.GetValueOrDefault(0M) * i.UnitPrice.GetValueOrDefault(0M)));
            invoice.HandWritten = BoYesNoEnum.tNO;
            invoice.DocNum = Convert.ToInt32(transaction.InvoiceNumber);


            if (transaction.InvoiceDate.HasValue)
            {

                invoice.DocDate = transaction.InvoiceDate.Value;
                invoice.TaxDate = transaction.InvoiceDate.Value;
            }


            if (transaction.InvoiceDueDate.HasValue)
                invoice.DocDueDate = transaction.InvoiceDueDate.Value;



            int lineNo = 0;
            foreach (InvoiceDetail item in transaction.Items)
            {

                Items vItem = (Items)_company.GetBusinessObject(BoObjectTypes.oItems);
                if (!vItem.GetByKey(item.ItemNumber.ToString())) CreateNonInventoryItem(item);
                var lines = invoice.Lines;
                lines.SetCurrentLine(lineNo);
                lines.ItemCode = item.ItemNumber;
                lines.ItemDescription = item.ItemDescription;
                lines.Quantity = Convert.ToDouble(item.QtyShipped);
                lines.UnitPrice = Convert.ToDouble(item.UnitPrice);
                lines.DiscountPercent = 0;
                lines.TaxType = BoTaxTypes.tt_No;
                lines.LineType = BoDocLineType.dlt_Regular;
                lines.Add();
            }

            if (invoice.Add() != 0)
            {
                string err = _company.GetLastErrorCode().ToString() + " - " + _company.GetLastErrorDescription();
                Logging.LogDebug($"failed to add Customer's Invoice { invoice.CardCode} {err}");
            }
            else
            {
                // mark AR as processed

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