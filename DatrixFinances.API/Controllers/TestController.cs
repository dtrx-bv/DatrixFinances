using DatrixFinances.API.Models;
using DatrixFinances.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatrixFinances.API.Controllers;

[ApiController]
[Route("test")]
[Produces("application/json")]
public class TestController : ControllerBase
{
    [Authorize]
    [HttpGet("authorized")]
    public async Task<ActionResult> Test()
    {
        return Ok("Well done Ben, not that stupid after al!");
    }
}