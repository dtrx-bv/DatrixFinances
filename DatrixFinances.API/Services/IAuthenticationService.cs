namespace DatrixFinances.API.Services;

public interface IAuthenticationService
{
    Task<string> YukiGetSessionId(string category, string accessKey);
}