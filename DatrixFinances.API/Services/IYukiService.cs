using DatrixFinances.API.Models.Network.Request;
using DatrixFinances.API.Models.Network.Request.Children;

namespace DatrixFinances.API.Services;

public interface IYukiService
{
    Task<object> GetAvailableVATCodes(string bearer, string administrationName);
    Task<object> GetAvailableGlAccounts(string bearer, string administrationName);
    Task<object> GetDomains(string bearer);
    Task<object> GetAdministrations(string bearer);
    Task<object> GetAdministrationId(string sessionID, string administrationName);
    Task<object> UploadSalesInvoice(string bearer, string administrationName, bool autoCorrectEnabled, SalesInvoice invoice);
    Task<object> GetAllOutstandingDebtorInvoices(string bearer, string administrationName, bool includeBankTransactions = false, string sortOrder = "DateAsc");
    Task<object> GetSalesItems(string bearer, string administrationName);
    Task<object> SearchContacts(string bearer, string searchTerm);
    Task<object> AddContact(string bearer, UpdateContact contact);
    Task<object> UpdateContact(string bearer, string id, UpdateContact contact);
}