using System.Globalization;
using System.Xml.Linq;
using DatrixFinances.API.Models;
using DatrixFinances.API.Models.Network.Request;
using DatrixFinances.API.Models.Network.Request.Children;
using DatrixFinances.API.Models.Network.Response;
using DatrixFinances.API.Pages;

namespace DatrixFinances.API.Services;

public class XMLService : IXMLService
{
    private const string DATE_ONLY_FORMAT = "yyyy-MM-dd";
    private const string PROPERTY_ERROR_MESSAGE = "MISSING_ELEMENT_VALUE";

    private readonly XNamespace soapEnv = "http://schemas.xmlsoap.org/soap/envelope/";
    private readonly XNamespace they = "http://www.theyukicompany.com/";

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

    public XElement CreateSearchContactXML(string sessionId, string searchTerm)
    {
        var root = new XElement(soapEnv + "Envelope",
            new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
            new XAttribute(XNamespace.Xmlns + "they", "http://www.theyukicompany.com/"));

        root.Add(new XElement(soapEnv + "Header"));

        var body = new XElement(soapEnv + "Body",
            new XElement(they + "SearchContacts",
                new XElement(they + "sessionID", sessionId),
                new XElement(they + "searchOption", "All"),
                new XElement(they + "searchValue", searchTerm)
            )
        );

        root.Add(body);
        return root;
    }

    public XElement CreateAddContactXML(string sessionId, Models.Network.Request.UpdateContact contact)
    {
        var root = new XElement(soapEnv + "Envelope",
            new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
            new XAttribute(XNamespace.Xmlns + "they", "http://www.theyukicompany.com/"));

        root.Add(new XElement(soapEnv + "Header"));

        XNamespace contactNs = "urn:xmlns:http://www.theyukicompany.com:contact";

        var fullName = !string.IsNullOrWhiteSpace(contact.FullName)
            ? contact.FullName
            : string.Join(" ", new[] { contact.FirstName, contact.MiddleName, contact.LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

        var contactTypeValue = string.Equals(contact.Type, "Person", StringComparison.OrdinalIgnoreCase)
            ? 0
            : string.Equals(contact.Type, "Company", StringComparison.OrdinalIgnoreCase)
            ? 2
            : int.MinValue;

        var contactElement = new XElement(contactNs + "Contact",
            new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            new XElement(contactNs + "Type", contactTypeValue),
            new XElement(contactNs + "Code", contact.Code ?? string.Empty),
            new XElement(contactNs + "FirstName", contact.FirstName ?? string.Empty),
            new XElement(contactNs + "MiddleName", contact.MiddleName ?? string.Empty),
            new XElement(contactNs + "LastName", contact.LastName ?? string.Empty),
            new XElement(contactNs + "FullName", fullName ?? string.Empty),
            new XElement(contactNs + "AddressLine_1", contact.AddressLine1 ?? string.Empty),
            new XElement(contactNs + "AddressLine_2", contact.AddressLine2 ?? string.Empty),
            new XElement(contactNs + "PostCode", contact.PostCode ?? string.Empty),
            new XElement(contactNs + "City", contact.City ?? string.Empty),
            new XElement(contactNs + "MailAddressLine_1", contact.MailAddressLine1 ?? string.Empty),
            new XElement(contactNs + "MailAddressLine_2", contact.MailAddressLine2 ?? string.Empty),
            new XElement(contactNs + "MailPostCode", contact.MailPostCode ?? string.Empty),
            new XElement(contactNs + "MailCity", contact.MailCity ?? string.Empty),
            new XElement(contactNs + "Country", contact.Country ?? string.Empty),
            new XElement(contactNs + "WorkAddressLine_1", contact.WorkAddressLine1 ?? string.Empty),
            new XElement(contactNs + "WorkAddressLine_2", contact.WorkAddressLine2 ?? string.Empty),
            new XElement(contactNs + "WorkPostCode", contact.WorkPostCode ?? string.Empty),
            new XElement(contactNs + "WorkCity", contact.WorkCity ?? string.Empty),
            new XElement(contactNs + "WorkCountry", contact.WorkCountry ?? string.Empty),
            new XElement(contactNs + "PhoneHome", contact.PhoneHome ?? string.Empty),
            new XElement(contactNs + "PhoneWork", contact.PhoneWork ?? string.Empty),
            new XElement(contactNs + "MobileHome", contact.MobileHome ?? string.Empty),
            new XElement(contactNs + "MobileWork", contact.MobileWork ?? string.Empty),
            new XElement(contactNs + "EmailHome", contact.EmailHome ?? string.Empty),
            new XElement(contactNs + "EmailWork", contact.EmailWork ?? string.Empty),
            new XElement(contactNs + "VATNumber", contact.VATNumber ?? string.Empty),
            new XElement(contactNs + "CoCNumber", contact.CocNumber ?? string.Empty)
        );

        var body = new XElement(soapEnv + "Body",
            new XElement(they + "UpdateContact",
                new XElement(they + "sessionID", sessionId),
                new XElement(they + "xmlDoc", contactElement)
            )
        );

        root.Add(body);
        return root;
    }

    public XElement CreateUpdateContactXML(string sessionId, string id, Models.Network.Request.UpdateContact contact)
    {
        var root = new XElement(soapEnv + "Envelope",
            new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
            new XAttribute(XNamespace.Xmlns + "they", "http://www.theyukicompany.com/"));

        root.Add(new XElement(soapEnv + "Header"));

        XNamespace contactNs = "urn:xmlns:http://www.theyukicompany.com:contact";

        var fullName = !string.IsNullOrWhiteSpace(contact.FullName)
            ? contact.FullName
            : string.Join(" ", new[] { contact.FirstName, contact.MiddleName, contact.LastName }
                .Where(s => !string.IsNullOrWhiteSpace(s)));

        var contactTypeValue = string.Equals(contact.Type, "Person", StringComparison.OrdinalIgnoreCase)
            ? 0
            : string.Equals(contact.Type, "Company", StringComparison.OrdinalIgnoreCase)
            ? 2
            : int.MinValue;

        var contactElement = new XElement(contactNs + "Contact",
            new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            new XElement(contactNs + "ID", id ?? string.Empty),
            new XElement(contactNs + "Type", contactTypeValue),
            new XElement(contactNs + "Code", contact.Code ?? string.Empty),
            new XElement(contactNs + "FirstName", contact.FirstName ?? string.Empty),
            new XElement(contactNs + "MiddleName", contact.MiddleName ?? string.Empty),
            new XElement(contactNs + "LastName", contact.LastName ?? string.Empty),
            new XElement(contactNs + "FullName", fullName ?? string.Empty),
            new XElement(contactNs + "AddressLine_1", contact.AddressLine1 ?? string.Empty),
            new XElement(contactNs + "AddressLine_2", contact.AddressLine2 ?? string.Empty),
            new XElement(contactNs + "PostCode", contact.PostCode ?? string.Empty),
            new XElement(contactNs + "City", contact.City ?? string.Empty),
            new XElement(contactNs + "MailAddressLine_1", contact.MailAddressLine1 ?? string.Empty),
            new XElement(contactNs + "MailAddressLine_2", contact.MailAddressLine2 ?? string.Empty),
            new XElement(contactNs + "MailPostCode", contact.MailPostCode ?? string.Empty),
            new XElement(contactNs + "MailCity", contact.MailCity ?? string.Empty),
            new XElement(contactNs + "Country", contact.Country ?? string.Empty),
            new XElement(contactNs + "WorkAddressLine_1", contact.WorkAddressLine1 ?? string.Empty),
            new XElement(contactNs + "WorkAddressLine_2", contact.WorkAddressLine2 ?? string.Empty),
            new XElement(contactNs + "WorkPostCode", contact.WorkPostCode ?? string.Empty),
            new XElement(contactNs + "WorkCity", contact.WorkCity ?? string.Empty),
            new XElement(contactNs + "WorkCountry", contact.WorkCountry ?? string.Empty),
            new XElement(contactNs + "PhoneHome", contact.PhoneHome ?? string.Empty),
            new XElement(contactNs + "PhoneWork", contact.PhoneWork ?? string.Empty),
            new XElement(contactNs + "MobileHome", contact.MobileHome ?? string.Empty),
            new XElement(contactNs + "MobileWork", contact.MobileWork ?? string.Empty),
            new XElement(contactNs + "EmailHome", contact.EmailHome ?? string.Empty),
            new XElement(contactNs + "EmailWork", contact.EmailWork ?? string.Empty),
            new XElement(contactNs + "VATNumber", contact.VATNumber ?? string.Empty),
            new XElement(contactNs + "CoCNumber", contact.CocNumber ?? string.Empty)
        );

        var body = new XElement(soapEnv + "Body",
            new XElement(they + "UpdateContact",
                new XElement(they + "sessionID", sessionId),
                new XElement(they + "xmlDoc", contactElement)
            )
        );

        root.Add(body);
        return root;
    }

    public Models.Network.Response.UpdateContact ParseYukiUpdateContactResponse(string xml)
    {
        var doc = XDocument.Parse(xml);

        var response = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "ContactResponse")
            ?? throw new InvalidOperationException("Could not find ContactResponse node in XML.\n\n" + xml);

        var timeStampValue = response.Element(XName.Get("TimeStamp", ""))?.Value.Trim() ?? string.Empty;
        var succeededValue = response.Element(XName.Get("Succeeded", ""))?.Value.Trim() ?? string.Empty;

        var failedValues = response.Elements()
            .Where(e => e.Name.LocalName == "Failed")
            .Select(e => e.Value.Trim())
            .Where(v => !string.IsNullOrWhiteSpace(v))
            .ToList();

        return new Models.Network.Response.UpdateContact
        {
            TimeStamp = DateOnly.TryParse(timeStampValue, out var timeStamp) ? timeStamp : DateOnly.MinValue,
            Succeeded = succeededValue,
            Failed = failedValues
        };
    }

    public List<Models.Network.Response.Contact> ParseYukiContactResponseList(string xml)
    {
        var doc = XDocument.Parse(xml);

        return [.. doc.Descendants("Contact")
            .Select(contact =>
            {
                return new Models.Network.Response.Contact
                {
                    ContactId = (string?)contact.Attribute("ID") ?? string.Empty,
                    Type = (string?)contact.Element("Type") ?? string.Empty,
                    HID = int.TryParse((string?)contact.Element("HID"), out var hid) ? hid : int.MinValue,
                    Code = (string?)contact.Element("Code") ?? string.Empty,
                    Name = (string?)contact.Element("Name") ?? string.Empty,
                    FirstName = (string?)contact.Element("FirstName") ?? string.Empty,
                    MiddleName = (string?)contact.Element("MiddleName") ?? string.Empty,
                    LastName = (string?)contact.Element("LastName") ?? string.Empty,
                    AddressLine1 = (string?)contact.Element("AddressLine_1") ?? string.Empty,
                    AddressLine2 = (string?)contact.Element("AddressLine_2") ?? string.Empty,
                    PostCode = (string?)contact.Element("Postcode") ?? string.Empty,
                    City = (string?)contact.Element("City") ?? string.Empty,
                    MailAddressLine1 = (string?)contact.Element("MailAddressLine_1") ?? string.Empty,
                    MailAddressLine2 = (string?)contact.Element("MailAddressLine_2") ?? string.Empty,
                    MailPostCode = (string?)contact.Element("MailPostcode") ?? string.Empty,
                    MailCity = (string?)contact.Element("MailCity") ?? string.Empty,
                    Country = (string?)contact.Element("Country") ?? string.Empty,
                    PhoneHome = (string?)contact.Element("PhoneHome") ?? string.Empty,
                    EmailWork = (string?)contact.Element("EMailWork") ?? string.Empty,
                    Website = (string?)contact.Element("Website") ?? string.Empty,
                    VATNumber = (string?)contact.Element("VATNumber") ?? string.Empty,
                    CocNumber = (string?)contact.Element("CoCNumber") ?? string.Empty,
                    IncomeTaxNumber = (string?)contact.Element("IncomeTaxNumber") ?? string.Empty,
                    Created = DateTime.TryParseExact((string?)contact.Element("Created"), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var created) ? created : DateTime.MinValue,
                    Modified = DateTime.TryParseExact((string?)contact.Element("Modified"), "M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var modified) ? modified : DateTime.MinValue,
                    MainContactPerson = (string?)contact.Element("MainContactPerson") ?? string.Empty,
                    IsSupplier = bool.TryParse((string?)contact.Element("IsSupplier"), out var isSupplier) && isSupplier,
                    IsCustomer = bool.TryParse((string?)contact.Element("IsCustomer"), out var isCustomer) && isCustomer,
                    IBAN = (string?)contact.Element("IBAN") ?? string.Empty,
                    BIC = (string?)contact.Element("BIC") ?? string.Empty,
                    Tags = (string?)contact.Element("Tags") ?? string.Empty,
                    BackofficeStatus = (string?)contact.Element("BackofficeStatus") ?? string.Empty
                };
            })];
    }

    public List<Domain> ParseYukiDomainResponse(string xml)
    {
        var doc = XDocument.Parse(xml);

        return doc.Root?
            .Elements("Domain")
            .Select(domainElement => new Domain
            {
                Id = (string)domainElement.Attribute("ID")!,
                Name = (string)domainElement.Element("Name")!,
                URL = (string)domainElement.Element("URL")!,
                StatusId = (string)domainElement.Element("Status")?.Attribute("ID")!,
                Status = (string)domainElement.Element("Status")!
            })
            .ToList() ?? [];
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

        return [.. doc.Descendants(ns + "GlAccount")
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
            })];
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
        var doc = XDocument.Parse(xml);

        var response = doc.Descendants()
            .FirstOrDefault(e => e.Name.LocalName == "SalesInvoicesImportResponse") ?? throw new InvalidOperationException("Could not find SalesInvoicesImportResponse node in XML.");

        return new ProcessSalesInvoice
        {
            TimeStamp = DateOnly.Parse(response.Element(XName.Get("TimeStamp", ""))?.Value.Trim() ?? DateOnly.MinValue.ToString()),
            AdministrationId = response.Element(XName.Get("AdministrationId", ""))?.Value.Trim() ?? string.Empty,
            TotalSucceeded = int.Parse(response.Element(XName.Get("TotalSucceeded", ""))?.Value.Trim() ?? "0"),
            TotalFailed = int.Parse(response.Element(XName.Get("TotalFailed", ""))?.Value.Trim() ?? "0"),
            TotalSkipped = int.Parse(response.Element(XName.Get("TotalSkipped", ""))?.Value.Trim() ?? "0"),
            Invoices = [.. response.Elements(XName.Get("Invoice", ""))
                .Select(inv => new ProcessSalesInvoice.Invoice
                {
                    Succeeded = bool.Parse(inv.Element("Succeeded")?.Value.Trim() ?? "false"),
                    Processed = bool.Parse(inv.Element("Processed")?.Value.Trim() ?? "false"),
                    EmailSent = bool.Parse(inv.Element("EmailSent")?.Value.Trim() ?? "false"),
                    Reference = inv.Element("Reference")?.Value.Trim() ?? string.Empty,
                    Subject = inv.Element("Subject")?.Value.Trim() ?? string.Empty,
                    Contact = inv.Element("Contact")?.Value.Trim() ?? string.Empty,
                    Message = inv.Element("Message")?.Value.Trim() ?? string.Empty
                })]
        };;
    }

    public XElement CreateRequestXMLYukiProcessSalesInvoice(string sessionId, string administrationId, bool disableAutoCorrect, SalesInvoice invoice)
    {
        var root = new XElement(soapEnv + "Envelope",
            new XAttribute(XNamespace.Xmlns + "soapenv", "http://schemas.xmlsoap.org/soap/envelope/"),
            new XAttribute(XNamespace.Xmlns + "they", "http://www.theyukicompany.com/"));

        root.Add(new XElement(soapEnv + "Header"));

        XNamespace nsYuki = "urn:xmlns:http://www.theyukicompany.com:salesinvoices";
        var doc = new XElement(nsYuki + "SalesInvoices",
            new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
            new XAttribute(XNamespace.Xmlns + "xsd", "http://www.w3.org/2001/XMLSchema"));

        var childElement = new XElement(nsYuki + "SalesInvoice");
        childElement.Add(new XElement(nsYuki + "Reference", !string.IsNullOrEmpty(invoice.Reference) ? invoice.Reference : PROPERTY_ERROR_MESSAGE));
        childElement.Add(new XElement(nsYuki + "Subject", !string.IsNullOrEmpty(invoice.Subject) ? invoice.Subject : PROPERTY_ERROR_MESSAGE));
        childElement.Add(new XElement(nsYuki + "PaymentMethod", !string.IsNullOrEmpty(invoice.PaymentMethod) ? invoice.PaymentMethod : PROPERTY_ERROR_MESSAGE));
        childElement.Add(new XElement(nsYuki + "PaymentID", !string.IsNullOrEmpty(invoice.PaymentID) ? invoice.PaymentID : PROPERTY_ERROR_MESSAGE));
        childElement.Add(new XElement(nsYuki + "Process", invoice.Process));
        childElement.Add(new XElement(nsYuki + "EmailToCustomer", invoice.EmailToCustomer));
        childElement.Add(new XElement(nsYuki + "SentToPeppol", invoice.SentToPeppol));

        if (string.IsNullOrEmpty(invoice.Layout))
            childElement.Add(new XElement(nsYuki + "Layout"));
        else
            childElement.Add(new XElement(nsYuki + "Layout", invoice.Layout));

        childElement.Add(new XElement(nsYuki + "Date", invoice.Date.CompareTo(DateOnly.MinValue) == 1 ? invoice.Date.ToString(DATE_ONLY_FORMAT) : PROPERTY_ERROR_MESSAGE));
        childElement.Add(new XElement(nsYuki + "DueDate", invoice.DueDate.CompareTo(DateOnly.MinValue) == 1 ? invoice.Date.ToString(DATE_ONLY_FORMAT) : PROPERTY_ERROR_MESSAGE));

        if (string.IsNullOrEmpty(invoice.PriceList))
            childElement.Add(new XElement(nsYuki + "PriceList"));
        else
            childElement.Add(new XElement(nsYuki + "PriceList", invoice.PriceList));

        if (string.IsNullOrEmpty(invoice.Currency))
            childElement.Add(new XElement(nsYuki + "Currency"));
        else
            childElement.Add(new XElement(nsYuki + "Currency", invoice.Currency));

        if (string.IsNullOrEmpty(invoice.ProjectID))
            childElement.Add(new XElement(nsYuki + "ProjectID"));
        else
            childElement.Add(new XElement(nsYuki + "ProjectID", invoice.ProjectID));

        if (string.IsNullOrEmpty(invoice.ProjectCode))
            childElement.Add(new XElement(nsYuki + "ProjectCode"));
        else
            childElement.Add(new XElement(nsYuki + "ProjectCode", invoice.ProjectCode));

        if (string.IsNullOrEmpty(invoice.Remarks))
            childElement.Add(new XElement(nsYuki + "Remarks"));
        else
            childElement.Add(new XElement(nsYuki + "Remarks", invoice.Remarks));

        var contact = new XElement(nsYuki + "Contact");

        if (string.IsNullOrEmpty(invoice.Contact.ContactCode))
            contact.Add(new XElement(nsYuki + "ContactCode"));
        else
            contact.Add(new XElement(nsYuki + "ContactCode", invoice.Contact.ContactCode));

        contact.Add(new XElement(nsYuki + "FullName", !string.IsNullOrEmpty(invoice.Contact.FullName) ? invoice.Contact.FullName : PROPERTY_ERROR_MESSAGE));

        if (string.IsNullOrEmpty(invoice.Contact.FirstName))
            contact.Add(new XElement(nsYuki + "FirstName"));
        else
            contact.Add(new XElement(nsYuki + "FirstName", invoice.Contact.FirstName));

        if (string.IsNullOrEmpty(invoice.Contact.MiddleName))
            contact.Add(new XElement(nsYuki + "MiddleName"));
        else
            contact.Add(new XElement(nsYuki + "MiddleName", invoice.Contact.MiddleName));

        if (string.IsNullOrEmpty(invoice.Contact.LastName))
            contact.Add(new XElement(nsYuki + "LastName"));
        else
            contact.Add(new XElement(nsYuki + "LastName", invoice.Contact.LastName));

        contact.Add(new XElement(nsYuki + "Gender", !string.IsNullOrEmpty(invoice.Contact.Gender) ? invoice.Contact.Gender : PROPERTY_ERROR_MESSAGE));
        contact.Add(new XElement(nsYuki + "CountryCode", !string.IsNullOrEmpty(invoice.Contact.CountryCode) ? invoice.Contact.CountryCode : PROPERTY_ERROR_MESSAGE));
        contact.Add(new XElement(nsYuki + "City", !string.IsNullOrEmpty(invoice.Contact.City) ? invoice.Contact.City : PROPERTY_ERROR_MESSAGE));

        if (string.IsNullOrEmpty(invoice.Contact.Zipcode))
            contact.Add(new XElement(nsYuki + "Zipcode"));
        else
            contact.Add(new XElement(nsYuki + "Zipcode", invoice.Contact.Zipcode));

        contact.Add(new XElement(nsYuki + "AddressLine_1", !string.IsNullOrEmpty(invoice.Contact.AddressLine1) ? invoice.Contact.AddressLine1 : PROPERTY_ERROR_MESSAGE));

        if (string.IsNullOrEmpty(invoice.Contact.AddressLine2))
            contact.Add(new XElement(nsYuki + "AddressLine_2"));
        else
            contact.Add(new XElement(nsYuki + "AddressLine_2", invoice.Contact.AddressLine2));

        if (string.IsNullOrEmpty(invoice.Contact.Website))
            contact.Add(new XElement(nsYuki + "Website"));
        else
            contact.Add(new XElement(nsYuki + "Website", invoice.Contact.Website));

        if (string.IsNullOrEmpty(invoice.Contact.CocNumber))
            contact.Add(new XElement(nsYuki + "CoCNumber"));
        else
            contact.Add(new XElement(nsYuki + "CoCNumber", invoice.Contact.CocNumber));

        if (string.IsNullOrEmpty(invoice.Contact.VATNumber))
            contact.Add(new XElement(nsYuki + "VATNumber"));
        else
            contact.Add(new XElement(nsYuki + "VATNumber", invoice.Contact.VATNumber));

        contact.Add(new XElement(nsYuki + "ContactType", !string.IsNullOrEmpty(invoice.Contact.ContactType) ? invoice.Contact.ContactType : PROPERTY_ERROR_MESSAGE));

        if (string.IsNullOrEmpty(invoice.Contact.BankAccount))
            contact.Add(new XElement(nsYuki + "BankAccount"));
        else
            contact.Add(new XElement(nsYuki + "BankAccount", invoice.Contact.BankAccount));

        if (string.IsNullOrEmpty(invoice.Contact.PhoneHome))
            contact.Add(new XElement(nsYuki + "PhoneHome"));
        else
            contact.Add(new XElement(nsYuki + "PhoneHome", invoice.Contact.PhoneHome));

        if (string.IsNullOrEmpty(invoice.Contact.MobileHome))
            contact.Add(new XElement(nsYuki + "MobileHome"));
        else
            contact.Add(new XElement(nsYuki + "MobileHome", invoice.Contact.MobileHome));

        childElement.Add(contact); 

        var invoiceLines = new XElement(nsYuki + "InvoiceLines");

        if (invoice.InvoiceLines.Count == 0)
        {
            invoiceLines.Add(
                new XElement(nsYuki + "InvoiceLine",
                    new XElement(nsYuki + "Description", PROPERTY_ERROR_MESSAGE),
                    new XElement(nsYuki + "ProductQuantity", PROPERTY_ERROR_MESSAGE),
                    new XElement(nsYuki + "Product",
                        new XElement(nsYuki + "Description", PROPERTY_ERROR_MESSAGE),
                        new XElement(nsYuki + "Reference", PROPERTY_ERROR_MESSAGE),
                        new XElement(nsYuki + "SalesPrice", PROPERTY_ERROR_MESSAGE),
                        new XElement(nsYuki + "VATPercentage", PROPERTY_ERROR_MESSAGE),
                        new XElement(nsYuki + "VATIncluded", PROPERTY_ERROR_MESSAGE),
                        new XElement(nsYuki + "VATType", PROPERTY_ERROR_MESSAGE)
                    )
                )
            );
        }
        else
        {
            invoice.InvoiceLines.ForEach(invoiceLine =>
            {
                var xmlInvoiceLine = new XElement(nsYuki + "InvoiceLine");
                xmlInvoiceLine.Add(
                    new XElement(nsYuki + "Description", !string.IsNullOrEmpty(invoiceLine.Description) ? invoiceLine.Description : PROPERTY_ERROR_MESSAGE),
                    new XElement(nsYuki + "ProductQuantity", invoiceLine.ProductQuantity != double.MinValue ? invoiceLine.ProductQuantity : PROPERTY_ERROR_MESSAGE)
                );

                if (invoiceLine.LineAmount != double.MinValue)
                    xmlInvoiceLine.Add(new XElement(nsYuki + "LineAmount", invoiceLine.LineAmount));

                if (invoiceLine.LineVATAmount != double.MinValue)
                    xmlInvoiceLine.Add(new XElement(nsYuki + "LineVATAmount", invoiceLine.LineVATAmount));

                var product = new XElement(nsYuki + "Product");

                product.Add(new XElement(nsYuki + "Description", !string.IsNullOrEmpty(invoiceLine.Product.Description) ? invoiceLine.Product.Description : PROPERTY_ERROR_MESSAGE));
                product.Add(new XElement(nsYuki + "Reference", !string.IsNullOrEmpty(invoiceLine.Product.Reference) ? invoiceLine.Product.Reference : PROPERTY_ERROR_MESSAGE));
                product.Add(new XElement(nsYuki + "SalesPrice", invoiceLine.Product.SalesPrice != double.MinValue ? invoiceLine.Product.SalesPrice : PROPERTY_ERROR_MESSAGE));
                product.Add(new XElement(nsYuki + "VATPercentage", invoiceLine.Product.VATPercentage != double.MinValue ? invoiceLine.Product.VATPercentage : PROPERTY_ERROR_MESSAGE));
                product.Add(new XElement(nsYuki + "VATIncluded", invoiceLine.Product.VATIncluded));
                product.Add(new XElement(nsYuki + "VATType", invoiceLine.Product.VatType != double.MinValue ? invoiceLine.Product.VatType : PROPERTY_ERROR_MESSAGE));

                if (!string.IsNullOrEmpty(invoiceLine.Product.GLAccountCode))
                    product.Add(new XElement(nsYuki + "GLAccountCode", invoiceLine.Product.GLAccountCode));
                else
                    product.Add(new XElement(nsYuki + "VATDescription"));

                if (!string.IsNullOrEmpty(invoiceLine.Product.VatDescription) && string.IsNullOrEmpty(invoiceLine.Product.GLAccountCode))
                    product.Add(new XElement(nsYuki + "VATDescription", invoiceLine.Product.VatDescription));

                xmlInvoiceLine.Add(product);
                invoiceLines.Add(xmlInvoiceLine);
            });
        }

        childElement.Add(invoiceLines);

        doc.Add(childElement);

        var body = new XElement(soapEnv + "Body",
            new XElement(they + "ProcessSalesInvoices",
                new XElement(they + "sessionId", sessionId),
                new XElement(they + "administrationId", administrationId),
                new XElement(they + "disableAutoCorrect", disableAutoCorrect),
                new XElement(they + "xmlDoc", doc)
            )
        );

        root.Add(body);
        return root;
    }
}