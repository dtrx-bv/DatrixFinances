using System.Xml.Linq;
using DatrixFinances.API.Models;
using DatrixFinances.API.Models.Network.Request;
using DatrixFinances.API.Models.Network.Response;

namespace DatrixFinances.API.Services;

public interface IXMLService
{
    ErrorResponse ParseYukiErrorResponse(string xml);
    List<Administration> ParseYukiAdministrationResponse(string administrationsXML);
    List<OutstandingDebtorItem> ParseYukiOutstandingDebtorResponse(string xml);
    List<YukiVATCode> ParseYukiVATCodeResponseList(string xml);
    List<GlAccount> ParseYukiGlAccountResponseList(string xml);
    List<SalesItem> ParseYukiSalesItemResponseList(string xml);
    ProcessSalesInvoice ParseYukiProcessSalesInvoicesResponse(string xml);
    XElement CreateRequestXMLYukiProcessSalesInvoice(string sessionId, string administrationId, bool disableAutoCorrect, List<SalesInvoice> invoices);
}