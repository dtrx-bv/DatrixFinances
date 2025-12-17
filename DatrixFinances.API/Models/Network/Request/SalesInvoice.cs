using DatrixFinances.API.Models.Network.Request.Children;

namespace DatrixFinances.API.Models.Network.Request;

public class SalesInvoice
{
    public string Reference { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string PaymentID { get; set; } = string.Empty;
    public bool Process { get; set; }
    public bool EmailToCustomer { get; set; }
    public bool SentToPeppol { get; set; }
    public string Layout { get; set; } = string.Empty;
    public DateOnly Date { get; set; } = DateOnly.MinValue;
    public DateOnly DueDate { get; set; } = DateOnly.MinValue;
    public string PriceList { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
    public string ProjectID { get; set; } = string.Empty;
    public string ProjectCode { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public SalesContact Contact { get; set; } = new();
    public List<SalesInvoiceLine> InvoiceLines { get; set; } = [];
}