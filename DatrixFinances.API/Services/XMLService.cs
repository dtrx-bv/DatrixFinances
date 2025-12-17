using System.Globalization;
using System.Xml.Linq;
using DatrixFinances.API.Models;
using DatrixFinances.API.Models.Network.Request;
using DatrixFinances.API.Models.Network.Response;

namespace DatrixFinances.API.Services;

public class XMLService : IXMLService
{
    public ErrorResponse ParseYukiErrorResponse(string xml)
    {
        var doc = XDocument.Parse(xml);

        var fault = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "Fault") ?? throw new InvalidOperationException("Could not find <soap:Fault> node in XML.");

        var code = fault.Element("faultcode")?.Value.Trim() ?? string.Empty;
        var message = fault.Element("faultstring")?.Value.Trim() ?? string.Empty;

        // var details = message
        //     .Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries)
        //     .Select(d => d.Trim())
        //     .ToList();

        return new ErrorResponse
        {
            Code = code,
            Message = message//,
            //Details = details
        };
    }

    public List<Administration> ParseYukiAdministrationResponse(string administrationsXML)
    {
        var xDoc = XDocument.Parse(administrationsXML);

        return xDoc.Root?
            .Elements("Administration")
            .Select(adminElement => new Administration
            {
                Id = (string)adminElement.Attribute("ID")!,
                Name = (string)adminElement.Element("Name")!,
                AddressLine = (string)adminElement.Element("AddressLine")!,
                Postcode = (string)adminElement.Element("Postcode")!,
                City = (string)adminElement.Element("City")!,
                Country = (string)adminElement.Element("Country")!,
                CocNumber = (string)adminElement.Element("CoCNumber")!,
                VATNumber = (string)adminElement.Element("VATNumber")!,
                StartDate = DateTime.Parse((string)adminElement.Element("StartDate")!),
                DomainID = (string)adminElement.Element("DomainID")!,
                Active = bool.Parse((string)adminElement.Element("Active")!)
            })
            .ToList()!;
    }

    public List<OutstandingDebtorItem> ParseYukiOutstandingDebtorResponse(string xml)
    {
        var doc = XDocument.Parse(xml);

        return [.. doc.Descendants("Item")
            .Select(item => new OutstandingDebtorItem
            {
                Id = (string?)item.Attribute("ID") ?? string.Empty,
                Description = (string?)item.Element("Description") ?? string.Empty,
                Contact = (string?)item.Element("Contact") ?? string.Empty,
                ContactId = (string?)item.Element("ContactID") ?? string.Empty,
                OpenAmount = double.TryParse((string?)item.Element("OpenAmount"), NumberStyles.Any, CultureInfo.InvariantCulture, out var openAmount) ? openAmount : double.MinValue,
                OriginalAmount = double.TryParse((string?)item.Element("OriginalAmount"), NumberStyles.Any, CultureInfo.InvariantCulture, out var originalAmount) ? originalAmount : double.MinValue,
                TypeId = int.TryParse((string?)item.Element("Type")?.Attribute("ID"), out var typeId) ? typeId : int.MinValue,
                TypeName = (string?)item.Element("Type") ?? string.Empty,
                Reference = (string?)item.Element("Reference") ?? string.Empty,
                DueDate = DateOnly.TryParse((string?)item.Element("DueDate"), out var dueDate) ? dueDate : DateOnly.MinValue,
                DocumentId = (string?)item.Element("DocumentID") ?? string.Empty,
                PaymentMethod = (string?)item.Element("PaymentMethod") ?? string.Empty,
                ContactCode = (string?)item.Element("ContactCode") ?? string.Empty,
                VATNumber = (string?)item.Element("VATNumber") ?? string.Empty,
                AddressLine1 = (string?)item.Element("AddressLine_1") ?? string.Empty,
                AddressLine2 = (string?)item.Element("AddressLine_2") ?? string.Empty,
                Postcode = (string?)item.Element("Postcode") ?? string.Empty,
                City = (string?)item.Element("City") ?? string.Empty,
                MailAddressLine1 = (string?)item.Element("MailAddressLine_1") ?? string.Empty,
                MailAddressLine2 = (string?)item.Element("MailAddressLine_2") ?? string.Empty,
                MailPostcode = (string?)item.Element("MailPostcode") ?? string.Empty,
                MailCity = (string?)item.Element("MailCity") ?? string.Empty,
                Country = (string?)item.Element("Country") ?? string.Empty,
                RecipientEmail = (string?)item.Element("RecipientEmail") ?? string.Empty,
                PhoneHome = (string?)item.Element("PhoneHome") ?? string.Empty,
                PhoneWork = (string?)item.Element("PhoneWork") ?? string.Empty,
                EmailHome = (string?)item.Element("EmailHome") ?? string.Empty,
                EmailWork = (string?)item.Element("EmailWork") ?? string.Empty,
                EmailInvoice = (string?)item.Element("EmailInvoice") ?? string.Empty,
                EmailReminder = (string?)item.Element("EmailReminder") ?? string.Empty
            })];
    }

    public List<YukiVATCode> ParseYukiVATCodeResponseList(string xml)
    {
        var doc = XDocument.Parse(xml);

        XNamespace ns = "http://www.theyukicompany.com/";

        return [.. doc.Descendants(ns + "VATCode")
            .Select(vatCode => new YukiVATCode
            {
                Description = (string)vatCode.Element(ns + "description")! ?? string.Empty,
                Type = int.TryParse((string)vatCode.Element(ns + "type")!, out var type) ? type : int.MinValue,
                TypeDescription = (string)vatCode.Element(ns + "typeDescription")! ?? string.Empty,
                Percentage = double.TryParse((string)vatCode.Element(ns + "percentage")!, NumberStyles.Any, CultureInfo.InvariantCulture, out var percentage) ? percentage : double.MinValue,
                Country = (string)vatCode.Element(ns + "country")! ?? null,
                StartDate = DateTimeOffset.TryParse((string)vatCode.Element(ns + "startDate")!, out var startDate) ? startDate : null,
                EndDate = DateTimeOffset.TryParse((string)vatCode.Element(ns + "endDate")!, out var endDate) ? endDate : null,
            })];
    }

    public List<GlAccount> ParseYukiGlAccountResponseList(string xml)
    {
        var doc = XDocument.Parse(xml);

        XNamespace ns = "http://www.theyukicompany.com/";

        var accounts = doc.Descendants(ns + "GlAccount")
            .Select(acc => new GlAccount
            {
                Code = (string?)acc.Element(ns + "code") ?? string.Empty,
                Type = int.TryParse((string?)acc.Element(ns + "type"), out var type) ? type : int.MinValue,
                Subtype = int.TryParse((string?)acc.Element(ns + "subtype"), out var subtype) ? subtype : int.MinValue,
                IsEnabled = bool.TryParse((string?)acc.Element(ns + "isEnabled"), out var isEnabled) && isEnabled,
                Descripton = (string?)acc.Element(ns + "descripton") ?? string.Empty,
                DescriptionNL = (string?)acc.Element(ns + "descriptionNL") ?? string.Empty,
                DescriptionFR = (string?)acc.Element(ns + "descriptionFR") ?? string.Empty,
                DescriptionEN = (string?)acc.Element(ns + "descriptionEN") ?? string.Empty,
                IsVATApplicable = bool.TryParse((string?)acc.Element(ns + "isVATApplicable"), out var vatApplicable) && vatApplicable,
                DeductableVATPercentage = double.TryParse((string?)acc.Element(ns + "deductableVATPercentage"), NumberStyles.Any, CultureInfo.InvariantCulture, out var deductable) ? deductable : double.MinValue,
                ProfessionalPercentage = double.TryParse((string?)acc.Element(ns + "professionalPercentage"), NumberStyles.Any, CultureInfo.InvariantCulture, out var professional) ? professional : double.MinValue
            })
            .ToList();

        return accounts;
    }

    public List<SalesItem> ParseYukiSalesItemResponseList(string xml)
    {
        var doc = XDocument.Parse(xml);

        XNamespace ns = "http://www.theyukicompany.com/";

        return [.. doc.Descendants(ns + "SalesItem")
            .Select(item => new SalesItem
            {
                Id = (string?)item.Element(ns + "id") ?? string.Empty,
                Description = (string?)item.Element(ns + "description") ?? string.Empty
            })];
    }

    public ProcessSalesInvoice ParseYukiProcessSalesInvoicesResponse(string xml)
    {
        throw new NotImplementedException();
    }

    public XElement CreateRequestXMLYukiProcessSalesInvoice(string sessionId, string administrationId, bool disableAutoCorrect, List<SalesInvoice> invoices)
    {
        throw new NotImplementedException();
    }
}