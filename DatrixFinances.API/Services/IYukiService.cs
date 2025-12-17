using DatrixFinances.API.Models.Network.Request;

namespace DatrixFinances.API.Services;

public interface IYukiService
{
    Task<object> GetAvailableVATCodes(string bearer, string administrationName);
    Task<object> GetAvailableGlAccounts(string bearer, string administrationName);
    Task<object> GetAdministrations(string bearer);
    Task<object> GetAdministrationId(string sessionID, string administrationName);
    Task<object> UploadSalesInvoice(string bearer, string administrationName, bool autoCorrectEnabled, List<SalesInvoice> invoices);
    Task<object> GetAllOutstandingDebtorInvoices(string bearer, string administrationName, bool includeBankTransactions = false, string sortOrder = "DateAsc");
    Task<object> GetSalesItems(string bearer, string administrationName);
}