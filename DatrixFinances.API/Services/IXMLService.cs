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
    XElement CreateRequestXMLYukiProcessSalesInvoice(string sessionId, string administrationId, bool disableAutoCorrect, SalesInvoice invoice);
    XElement CreateSearchContactXML(string sessionId, string searchTerm);
    List<Contact> ParseYukiContactResponseList(string xml);
    List<Domain> ParseYukiDomainResponse(string xml);
    XElement CreateAddContactXML(string sessionId, UpdateContact contact);
    XElement CreateUpdateContactXML(string sessionId, string id, UpdateContact contact);
    ContactResult ParseYukiUpdateContactResponse(string xml);
}