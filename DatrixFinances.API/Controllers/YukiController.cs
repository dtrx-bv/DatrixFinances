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
        if (authHeader == null || !authHeader.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var sessionID = authHeader["Apikey ".Length..].Trim();
        var response = await _yukiService.GetAllOutstandingDebtorInvoices(sessionID, administrationName);
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
    public async Task<ActionResult> UploadYukiSalesInvoice(string administrationName, List<SalesInvoice> invoices)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var sessionID = authHeader["Apikey ".Length..].Trim();
        var response = await _yukiService.UploadSalesInvoice(sessionID, administrationName, false, invoices);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    /// <summary>
    /// Returns all administrations that can be accessed with the given session ID.
    /// </summary>
    [Authorize]
    [HttpGet("company/administration")]
    public async Task<ActionResult> GetYukiAdministrations()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var sessionID = authHeader["Apikey ".Length..].Trim();
        var response = await _yukiService.GetAdministrations(sessionID);
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
    public async Task<ActionResult> GetYukiAvailableCompanyVATCodes(string administrationName)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var sessionID = authHeader["Apikey ".Length..].Trim();
        var response = await _yukiService.GetAvailableVATCodes(sessionID, administrationName);
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
        if (authHeader == null || !authHeader.StartsWith("Apikey ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var sessionID = authHeader["Apikey ".Length..].Trim();
        var response = await _yukiService.GetAvailableGlAccounts(sessionID, administrationName);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }
}