namespace DatrixFinances.API.Services;

public interface IAuthenticationService
{
    Task<object> GetBearerToken(string clientId, string clientSecret);
    Task<string> YukiGetSessionId(string category, string accessKey);
}