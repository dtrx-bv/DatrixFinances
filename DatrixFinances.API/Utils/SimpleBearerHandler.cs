using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DatrixFinances.API.Utils;

#pragma warning disable CS0618
public class SimpleBearerHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, DatabaseContext databaseContext) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
#pragma warning restore CS0618 
    private readonly DatabaseContext _databaseContext = databaseContext;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var header = Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ")) return AuthenticateResult.Fail("Missing bearer");

        var token = header["Bearer ".Length..];
        var client = await _databaseContext.Users.FirstOrDefaultAsync(c => c.Bearer == token && c.TokenExpiry > DateTime.UtcNow);

        if (client == null) return AuthenticateResult.Fail("Invalid token");

        var claims = new[] { new Claim(ClaimTypes.Name, client.ClientId) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        return AuthenticateResult.Success(ticket);
    }
}