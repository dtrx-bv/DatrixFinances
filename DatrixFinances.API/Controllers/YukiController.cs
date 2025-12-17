using DatrixFinances.API.Models;
using DatrixFinances.API.Models.Network.Request;
using DatrixFinances.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatrixFinances.API.Controllers;

[ApiController]
[Route("yuki")]
[Produces("application/json")]
public class YukiController(IHttpContextAccessor httpContextAccessor, IYukiService yukiService) : ControllerBase
{

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IYukiService _yukiService = yukiService;
    
    /// <summary>
    /// List of open items in Debtors list.
    /// </summary>
    /// <param name="administrationName"></param>
    [Authorize]
    [HttpGet("invoice/outstanding/debtor/{administrationName}")]
    public async Task<ActionResult> GetAllOutstandingDebtorYukiInvoices(string administrationName)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetAllOutstandingDebtorInvoices(bearer, administrationName);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    /// <summary>
    /// Processes XML documents containing sales invoices.
    /// </summary>
    /// <param name="administrationName"></param>
    /// <param name="invoices"></param>
    [Authorize]
    [HttpPost("invoice/sales/upload/{administrationName}")]
    public async Task<ActionResult> UploadSalesInvoice(string administrationName, List<SalesInvoice> invoices)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.UploadSalesInvoice(bearer, administrationName, false, invoices);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    /// <summary>
    /// Returns all administrations that can be accessed with the given session ID.
    /// </summary>
    [Authorize]
    [HttpGet("company/administrations")]
    public async Task<ActionResult> GetAdministrations()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetAdministrations(bearer);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    /// <summary>
    /// Returns all administration VATCodes.
    /// </summary>
    /// <remarks>
    /// A vat code is added to the list upon processing an invoice, if it did not exist in the first place.
    /// </remarks>
    [Authorize]
    [HttpGet("company/{administrationName}/vatcodes")]
    public async Task<ActionResult> GetAvailableCompanyVATCodes(string administrationName)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetAvailableVATCodes(bearer, administrationName);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }
    
    /// <summary>
    /// Returns all administration GlAccounts.
    /// </summary>
    [Authorize]
    [HttpGet("company/{administrationName}/glaccounts")]
    public async Task<ActionResult> GetYukiAvailableGlAccounts(string administrationName)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetAvailableGlAccounts(bearer, administrationName);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    [Authorize]
    [HttpGet("company/{administrationName}/salesitems")]
    public async Task<ActionResult> GetYukiSalesItems(string administrationName)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetSalesItems(bearer, administrationName);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }
}