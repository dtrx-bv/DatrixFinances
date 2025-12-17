namespace DatrixFinances.API.Models.Network.Request.Children;

public class Product
{
    public string Description { get; set; } = string.Empty;
    public string Reference { get; set; } = string.Empty;
    public double SalesPrice { get; set; } = double.MinValue;
    public double VATPercentage { get; set; } = double.MinValue;
    public bool VATIncluded { get; set; }
    public int VatType { get; set; } = int.MinValue;
    public string GLAccountCode { get; set; } = string.Empty;
    public string VatDescription { get; set; } = string.Empty;
}