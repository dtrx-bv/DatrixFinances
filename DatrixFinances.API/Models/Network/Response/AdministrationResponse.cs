namespace DatrixFinances.API.Models.Network.Response;

public class Administration
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string AddressLine { get; set; } = string.Empty;
    public string Postcode { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string CocNumber { get; set; } = string.Empty;
    public string VATNumber { get; set; } = string.Empty;
    public DateTime StartDate { get; set; } = DateTime.MinValue;
    public string DomainID { get; set; } = string.Empty;
    public bool Active { get; set; }
}