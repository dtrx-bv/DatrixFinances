using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
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
        var endpoint = Context.GetEndpoint();
        if (endpoint?.Metadata?.GetMetadata<IAllowAnonymous>() != null)
            return AuthenticateResult.NoResult();

        if (!Request.Headers.ContainsKey("Authorization"))
            return AuthenticateResult.NoResult();

        var header = Request.Headers.Authorization.ToString().Trim();

        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return AuthenticateResult.NoResult();

        var token = header["Bearer ".Length..];

        var client = await _databaseContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(c =>
                c.Bearer == token &&
                c.TokenExpiry > DateTime.UtcNow);

        if (client == null)
            return AuthenticateResult.Fail("Invalid token");

        var claims = new[] { new Claim(ClaimTypes.Name, client.ClientId) };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        return AuthenticateResult.Success(ticket);
    }
}