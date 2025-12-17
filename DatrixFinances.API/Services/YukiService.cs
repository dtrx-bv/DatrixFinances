
using System.Xml.Linq;
using DatrixFinances.API.Models;
using DatrixFinances.API.Models.Network.Request;

namespace DatrixFinances.API.Services;

public class YukiService(IHttpClientFactory httpClientFactory, IXMLService xmlService, IAuthenticationService authenticationService) : IYukiService
{
    private readonly HttpClient _httpClientYuki = httpClientFactory.CreateClient("Yuki");
    private readonly IXMLService _xmlService = xmlService;
    private readonly IAuthenticationService _authenticationService = authenticationService;

    public async Task<object> GetAdministrationId(string sessionID, string administrationName)
    {
        XNamespace yuki = "http://www.theyukicompany.com/";
        var response = await _httpClientYuki.GetAsync($"/ws/Accounting.asmx/AdministrationID?sessionID={sessionID}&administrationName={administrationName}");
        if (response.IsSuccessStatusCode)
        {
            return XDocument.Parse(await response.Content.ReadAsStringAsync()).Root!.Value;
        }
        return new ErrorResponse { Code = $"Error parsing administration name.", Message = $"'{administrationName}' does not exist." };
    }

    public async Task<object> GetAdministrations(string accessKey)
    {
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", accessKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{accessKey}'" };
        var administrations = await (await _httpClientYuki.GetAsync($"/ws/Accounting.asmx/Administrations?sessionID={sessionID}")).Content.ReadAsStringAsync();
        return _xmlService.ParseYukiAdministrationResponse(administrations);
    }

    public async Task<object> GetAvailableGlAccounts(string accessKey, string administrationName)
    {
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", accessKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{accessKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.GetAsync($"/ws/AccountingInfo.asmx/GetGLAccountScheme?sessionID={sessionID}&administrationID={administrationID}");
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiGlAccountResponseList(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> GetAvailableVATCodes(string accessKey, string administrationName)
    {
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", accessKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{accessKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.GetAsync($"/ws/vat.asmx/ActiveVATCodesList?sessionID={sessionID}&administrationID={administrationID}");
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiVATCodeResponseList(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> UploadSalesInvoice(string accessKey, string administrationName, bool autoCorrectEnabled, List<SalesInvoice> invoices)
    {
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", accessKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{accessKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.PostAsync("/ws/Sales.asmx", new StringContent(_xmlService.CreateRequestXMLYukiProcessSalesInvoice(sessionID, (string)administrationID, autoCorrectEnabled, invoices).ToString(SaveOptions.DisableFormatting), null, "text/xml"));
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiProcessSalesInvoicesResponse(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> GetAllOutstandingDebtorInvoices(string accessKey, string administrationName, bool includeBankTransactions = false, string sortOrder = "DateAsc")
    {
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", accessKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{accessKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.GetAsync($"/ws/Accounting.asmx/OutstandingDebtorItems?sessionID={sessionID}&administrationID={administrationID}&includeBankTransactions={includeBankTransactions}&sortOrder={sortOrder}");
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiOutstandingDebtorResponse(await response.Content.ReadAsStringAsync());
    }
}