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
    [HttpGet("domain")]
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
    [HttpGet("administration")]
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
    /// <param name="administration"></param>
    [Authorize]
    [HttpGet("administration/{administration}/invoice/outstanding/debtor")]
    public async Task<ActionResult> GetAllOutstandingDebtorInvoices(string administration)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetAllOutstandingDebtorInvoices(bearer, administration);
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
    /// <param name="administration"></param>
    /// <param name="invoice"></param>
    [Authorize]
    [HttpPost("/yuki/administration/{administration}/invoice/sales")]
    public async Task<ActionResult> UploadSalesInvoice(string administration, SalesInvoice invoice)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.UploadSalesInvoice(bearer, administration, false, invoice);
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
    [HttpGet("administration/{administration}/vatcode")]
    public async Task<ActionResult> GetAvailableCompanyVATCodes(string administration)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetAvailableVATCodes(bearer, administration);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }
    
    /// <summary>
    /// Returns all administration GlAccounts.
    /// </summary>
    [Authorize]
    [HttpGet("administration/{administration}/glaccount")]
    public async Task<ActionResult> GetYukiAvailableGlAccounts(string administration)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetAvailableGlAccounts(bearer, administration);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    /// <summary>
    /// Returns all sales items for the given administration.
    /// </summary>
    [Authorize]
    [HttpGet("administration/{administration}/salesitems")]
    public async Task<ActionResult> GetYukiSalesItems(string administration)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.GetSalesItems(bearer, administration);
        if (response is ErrorResponse)
            return UnprocessableEntity(response);
        return Ok(response);
    }

    /// <summary>
    /// Search Yuki contacts by search term.
    /// </summary>
    [Authorize]
    [HttpGet("contact/{query}")]
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

    /// <summary>
    /// Add Yuki contact.
    /// </summary>
    /// <remarks>
    /// Contact Schema can be found <a target="_blank" href="https://api.yukiworks.be/schemas/Contact.xsd">here</a>.
    /// </remarks>
    [Authorize]
    [HttpPost("contact/")]
    [Consumes("application/json")]
    public async Task<ActionResult> AddYukiContact(UpdateContact contact)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.AddContact(bearer, contact);
        if (response is ErrorResponse)
            return NotFound(response);
        return Ok(response);
    }

    /// <summary>
    /// Update Yuki contact.
    /// </summary>
    /// <remarks>
    /// Contact Schema can be found <a target="_blank" href="https://api.yukiworks.be/schemas/Contact.xsd">here</a>.
    /// </remarks>
    [Authorize]
    [HttpPut("contact/{id}")]
    [Consumes("application/json")]
    public async Task<ActionResult> UpdateYukiContact(string id, UpdateContact contact)
    {
        var authHeader = _httpContextAccessor.HttpContext?.Request.Headers.Authorization.FirstOrDefault();
        if (authHeader == null || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            return Unauthorized();
        var bearer = authHeader["Bearer ".Length..].Trim();
        var response = await _yukiService.UpdateContact(bearer, id, contact);
        if (response is ErrorResponse)
            return NotFound(response);
        if ((response as Models.Network.Response.UpdateContact)!.Failed.Count != 0)
            return UnprocessableEntity(response);
        return Ok(response);
    }
}