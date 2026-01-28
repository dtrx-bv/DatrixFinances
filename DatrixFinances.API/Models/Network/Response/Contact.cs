namespace DatrixFinances.API.Models.Network.Response;

public class Contact
{
    public string ContactId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int HID { get; set; } = int.MinValue;
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string MiddleName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string AddressLine1 { get; set; } = string.Empty;
    public string AddressLine2 { get; set; } = string.Empty;
    public string PostCode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string MailAddressLine1 { get; set; } = string.Empty;
    public string MailAddressLine2 { get; set; } = string.Empty;
    public string MailPostCode { get; set; } = string.Empty;
    public string MailCity { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string PhoneHome { get; set; } = string.Empty;
    public string EmailWork { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string VATNumber { get; set; } = string.Empty;
    public string CocNumber { get; set; } = string.Empty;
    public string IncomeTaxNumber { get; set; } = string.Empty;
    public DateTime Created { get; set; } = DateTime.MinValue;
    public DateTime Modified { get; set; } = DateTime.MinValue;
    public string MainContactPerson { get; set; } = string.Empty;
    public bool IsSupplier { get; set; } = false;
    public bool IsCustomer { get; set; } = false;
    public string IBAN { get; set; } = string.Empty;
    public string BIC { get; set; } = string.Empty;
    public string Tags { get; set; } = string.Empty;
    public string BackofficeStatus { get; set; } = string.Empty;
}