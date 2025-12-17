namespace DatrixFinances.API.Models.Network.Response;

public class ProcessSalesInvoice
{
    public DateOnly TimeStamp { get; set; } = DateOnly.MinValue;
    public string AdministrationId { get; set; } = string.Empty;
    public int TotalSucceeded { get; set; } = int.MinValue;
    public int TotalFailed { get; set; } = int.MinValue;
    public int TotalSkipped { get; set; } = int.MinValue;
    public List<Invoice> Invoices { get; set; } = [];

    public class Invoice
    {
        public bool Succeeded { get; set; }
        public bool Processed { get; set; }
        public bool EmailSent { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Contact { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }
}