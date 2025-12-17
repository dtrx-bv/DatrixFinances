using DatrixFinances.API.Models.Network.Request;

namespace DatrixFinances.API.Services;

public interface IYukiService
{
    Task<object> GetAvailableVATCodes(string accessKey, string administrationName);
    Task<object> GetAvailableGlAccounts(string accesskey, string administrationName);
    Task<object> GetAdministrations(string accessKey);
    Task<object> GetAdministrationId(string sessionID, string administrationName);
    Task<object> UploadSalesInvoice(string accessKey, string administrationName, bool autoCorrectEnabled, List<SalesInvoice> invoices);
    Task<object> GetAllOutstandingDebtorInvoices(string accessKey, string administrationName, bool includeBankTransactions = false, string sortOrder = "DateAsc");
}