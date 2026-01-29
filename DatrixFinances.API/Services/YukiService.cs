
using System.Net;
using System.Xml.Linq;
using DatrixFinances.API.Models;
using DatrixFinances.API.Models.Network.Request;
using DatrixFinances.API.Repositories;

namespace DatrixFinances.API.Services;

public class YukiService(IHttpClientFactory httpClientFactory, IXMLService xmlService, IAuthenticationService authenticationService, IUserRepository userRepository) : IYukiService
{
    private readonly HttpClient _httpClientYuki = httpClientFactory.CreateClient("Yuki");
    private readonly IXMLService _xmlService = xmlService;
    private readonly IAuthenticationService _authenticationService = authenticationService;
    private readonly IUserRepository _userRepository = userRepository;
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

    public async Task<object> GetAdministrations(string bearer)
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var administrations = await (await _httpClientYuki.GetAsync($"/ws/Accounting.asmx/Administrations?sessionID={sessionID}")).Content.ReadAsStringAsync();
        return _xmlService.ParseYukiAdministrationResponse(administrations);
    }

    public async Task<object> GetDomains(string bearer)
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var domains = await (await _httpClientYuki.GetAsync($"/ws/Accounting.asmx/Domains?sessionID={sessionID}")).Content.ReadAsStringAsync();
        return _xmlService.ParseYukiDomainResponse(domains);
    }

    public async Task<object> GetAvailableGlAccounts(string bearer, string administrationName)
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.GetAsync($"/ws/AccountingInfo.asmx/GetGLAccountScheme?sessionID={sessionID}&administrationID={administrationID}");
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiGlAccountResponseList(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> GetAvailableVATCodes(string bearer, string administrationName)
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.GetAsync($"/ws/vat.asmx/ActiveVATCodesList?sessionID={sessionID}&administrationID={administrationID}");
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiVATCodeResponseList(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> UploadSalesInvoice(string bearer, string administrationName, bool autoCorrectEnabled, SalesInvoice invoice)
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.PostAsync("/ws/Sales.asmx", new StringContent(_xmlService.CreateRequestXMLYukiProcessSalesInvoice(sessionID, (string)administrationID, autoCorrectEnabled, invoice).ToString(SaveOptions.DisableFormatting), null, "text/xml"));
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiProcessSalesInvoicesResponse(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> GetAllOutstandingDebtorInvoices(string bearer, string administrationName, bool includeBankTransactions = false, string sortOrder = "DateAsc")
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.GetAsync($"/ws/Accounting.asmx/OutstandingDebtorItems?sessionID={sessionID}&administrationID={administrationID}&includeBankTransactions={includeBankTransactions}&sortOrder={sortOrder}");
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiOutstandingDebtorResponse(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> GetSalesItems(string bearer, string administrationName)
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var administrationID = await GetAdministrationId(sessionID, administrationName);
        if (administrationID is ErrorResponse)
            return administrationID;
        var response = await _httpClientYuki.GetAsync($"/ws/Sales.asmx/GetSalesItems?sessionID={sessionID}&administrationID={administrationID}");
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiSalesItemResponseList(await response.Content.ReadAsStringAsync());
    }

    public async Task<object> SearchContacts(string bearer, string searchTerm)
    {
        var user = await _userRepository.GetUserByBearer(bearer);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Bearer token is invalid or expired." };
        var sessionID = await _authenticationService.YukiGetSessionId("Accounting", user.YukiApiKey);
        if (string.IsNullOrEmpty(sessionID))
            return new ErrorResponse { Code = "Invalid access key.", Message = $"Our partner is unable to process access key '{user.YukiApiKey}'" };
        var response = await _httpClientYuki.PostAsync($"/ws/Contact.asmx", new StringContent(_xmlService.CreateSearchContactXML(sessionID, searchTerm).ToString(SaveOptions.DisableFormatting), null, "text/xml"));
        if (!response.IsSuccessStatusCode)
            return _xmlService.ParseYukiErrorResponse(await response.Content.ReadAsStringAsync());
        return _xmlService.ParseYukiContactResponseList(await response.Content.ReadAsStringAsync());
    }
}