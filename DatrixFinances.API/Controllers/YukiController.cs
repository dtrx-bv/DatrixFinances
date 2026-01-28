using System.Xml.Linq;
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
    /// Returns all company domains.
    /// </summary>
    [Authorize]
    [HttpGet("company/domains")]
    public async Task<ActionResult> GetDomains()
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetDomains(bearer);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    /// <summary>
    /// Returns all company administrations.
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
    /// List of open items in Debtors list.
    /// </summary>
    /// <param name="administrationName"></param>
    [Authorize]
    [HttpGet("invoice/outstanding/debtor/{administrationName}")]
    public async Task<ActionResult> GetAllOutstandingDebtorInvoices(string administrationName)
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
    /// <remarks>
    /// SalesInvoice Schema can be found <a target="_blank" href="https://api.yukiworks.be/schemas/SalesInvoices.xsd">here</a>.
    /// </remarks>
    /// <param name="administrationName"></param>
    /// <param name="invoice"></param>
    [Authorize]
    [HttpPost("invoice/sales/upload/{administrationName}")]
    public async Task<ActionResult> UploadSalesInvoice(string administrationName, SalesInvoice invoice)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.UploadSalesInvoice(bearer, administrationName, false, invoice);
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

    /// <summary>
    /// Returns all sales items for the given administration.
    /// </summary>
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

    /// <summary>
    /// Search Yuki contacts by search term.
    /// </summary>
    [Authorize]
    [HttpGet("company/contacts/{query}")]
    public async Task<ActionResult> GetYukiContacts(string query)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.SearchContacts(bearer, query);
        if (response is ErrorResponse)
            return NotFound(response);
        return Ok(response);
    }
}