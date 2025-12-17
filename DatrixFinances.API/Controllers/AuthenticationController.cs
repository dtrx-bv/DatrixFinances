using DatrixFinances.API.Models;
using DatrixFinances.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace DatrixFinances.API.Controllers;

[ApiController]
[Route("oauth2")]
[Produces("application/json")]
public class AuthenticationController(IAuthenticationService authenticationService) : ControllerBase
{

    private readonly IAuthenticationService _authenticationService = authenticationService;

    /// <summary>
    /// Generates a bearer token for the given client credentials.
    /// </summary>
    /// <param name="clientId"></param>
    /// <param name="clientSecret"></param>
    /// <returns></returns>
    [HttpPost("token")]
    [Consumes("application/x-www-form-urlencoded")]
    public async Task<ActionResult> GetToken([FromForm(Name = "client_id")] string clientId, [FromForm(Name = "client_secret")] string clientSecret)
    {
        Console.WriteLine($"Received token request with client_id: {clientId} and client_secret: {clientSecret}");
        var token = await _authenticationService.GetBearerToken(clientId, clientSecret);
        if (token is ErrorResponse error)
            return Unauthorized(error);
        return Ok(token);
    }
}