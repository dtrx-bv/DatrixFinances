namespace DatrixFinances.API.Models.Network.Response;

public class OutstandingDebtorItem
{
    public string Id { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Contact { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
    public double OpenAmount { get; set; } = double.MinValue;
    public double OriginalAmount { get; set; } = double.MinValue;
    public int TypeId { get; set; } = int.MinValue;
    public string TypeName { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public DateOnly DueDate { get; set; } = DateOnly.MinValue;
    public string DocumentId { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public string ContactCode { get; set; } = string.Empty;
    public string VATNumber { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string MailAddressLine1 { get; set; } = string.Empty;
    public string MailAddressLine2 { get; set; } = string.Empty;
    public string MailPostcode { get; set; } = string.Empty;
    public string MailCity { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string RecipientEmail { get; set; } = string.Empty;
    public string PhoneHome { get; set; } = string.Empty;
    public string PhoneWork { get; set; } = string.Empty;
    public string EmailHome { get; set; } = string.Empty;
    public string EmailWork { get; set; } = string.Empty;
    public string EmailInvoice { get; set; } = string.Empty;
    public string EmailReminder { get; set; } = string.Empty;
}