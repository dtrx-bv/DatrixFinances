using System.Xml.Linq;

namespace DatrixFinances.API.Services;

public class AuthenticationService(IHttpClientFactory httpClientFactory) : IAuthenticationService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("YukiHttpClient");

    public async Task<string> YukiGetSessionId(string category, string accessKey)
    {
        var response = await _httpClient.GetAsync($"/ws/{category}.asmx/Authenticate?accessKey={accessKey}");
        if (response.IsSuccessStatusCode)
            return XDocument.Parse(await response.Content.ReadAsStringAsync()).Root!.Value;
        return string.Empty;
    }
}