namespace DatrixFinances.API.Models.Network.Request;

public class UpdateContact
{
    public string Type { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
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
    public string WorkAddressLine1 { get; set; } = string.Empty;
    public string WorkAddressLine2 { get; set; } = string.Empty;
    public string WorkPostCode { get; set; } = string.Empty;
    public string WorkCity { get; set; } = string.Empty;
    public string WorkCountry { get; set; } = string.Empty;
    public string PhoneHome { get; set; } = string.Empty;
    public string PhoneWork { get; set; } = string.Empty;
    public string MobileHome { get; set; } = string.Empty;
    public string MobileWork { get; set; } = string.Empty;
    public string EmailHome { get; set; } = string.Empty;
    public string EmailWork { get; set; } = string.Empty;
    public string VATNumber { get; set; } = string.Empty;
    public string CocNumber { get; set; } = string.Empty;
}