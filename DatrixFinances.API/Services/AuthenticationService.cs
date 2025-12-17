using System.Net;
using System.Xml.Linq;
using DatrixFinances.API.Models;
using DatrixFinances.API.Repositories;

namespace DatrixFinances.API.Services;

public class AuthenticationService(IHttpClientFactory httpClientFactory, IConfiguration configuration, IUserRepository userRepository) : IAuthenticationService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Yuki");
    private readonly IConfiguration _configuration = configuration;
    private readonly IUserRepository _userRepository = userRepository;

    public async Task<object> GetBearerToken(string clientId, string clientSecret)
    {
        var user = await _userRepository.GetUserByCredentials(clientId, clientSecret);
        if (user == null)
            return new ErrorResponse { Code = HttpStatusCode.Unauthorized.ToString(), Message = "Invalid client credentials." };

        var token = Guid.NewGuid().ToString("N");
        user.Bearer = token;
        user.TokenExpiry = DateTime.UtcNow.AddHours(1);
        await _userRepository.Update(user);

        return new
        {
            access_token = token,
            token_type = "Bearer",
            expires_in = 3600
        };
    }

    public async Task<string> YukiGetSessionId(string category, string accessKey)
    {
        var response = await _httpClient.GetAsync($"/ws/{category}.asmx/Authenticate?accessKey={accessKey}");
        if (response.IsSuccessStatusCode)
            return XDocument.Parse(await response.Content.ReadAsStringAsync()).Root!.Value;
        return string.Empty;
    }
}