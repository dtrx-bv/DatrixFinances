namespace DatrixFinances.API.Models.Network.Request.Children;

public class InvoiceLine
{
    public string Description { get; set; } = string.Empty;
    public string GLAccountCode { get; set; } = string.Empty;
    public double LineAmount { get; set; } = double.MinValue;
    public double LineVatPercentage { get; set; } = double.MinValue;
    public double LineVATType { get; set; } = double.MinValue;
}