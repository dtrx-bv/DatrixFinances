using System.Xml.Linq;
using DatrixFinances.API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using DatrixFinances.API.Models.Network.Request;

namespace DatrixFinances.API.Controllers;

[ApiController]
[Route("developer")]
[Produces("application/json")]
public class DeveloperController(IHttpContextAccessor httpContextAccessor, IXMLService xmlService) : ControllerBase
{

    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private readonly IXMLService _xmlService = xmlService;

    /// <summary>
    /// Convert Invoice JSON to XML. [Only used for testing purpose!]
    /// </summary>
    /// <param name="invoice"></param>
    /// <returns></returns>
    [HttpPost("convert-invoice-json-to-xml")]
    [Produces("application/xml")]
    public XElement ConvertInvoiceJSONToXML(SalesInvoice invoice)
    {
        return _xmlService.CreateRequestXMLYukiProcessSalesInvoice("DEVELOPMENT-TEST", "DEVELOPMENT-TEST", false, invoice);
    }
}