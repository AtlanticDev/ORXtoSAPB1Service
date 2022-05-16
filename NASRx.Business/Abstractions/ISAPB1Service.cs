using NASRx.Model;

namespace NASRx.Business.Abstractions
{
    public interface ISAPB1Service
    {
        bool CheckIfBusinessPartnerExist(string bpKey);

        (bool result, string errorMsg) Connect();

        void CreateBusinessPartner(Invoice invoice);

        void CreateInvoice(Invoice invoice);

        bool Disconnect();
    }
}