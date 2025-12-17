namespace DatrixFinances.API.Models.Network.Request.Children;

public class SalesInvoiceLine
{
    public string Description { get; set; } = string.Empty;
    public int ProductQuantity { get; set; } = int.MinValue;
    public double LineAmount { get; set; } = double.MinValue;
    public double LineVATAmount { get; set; } = double.MinValue;
    public Product Product { get; set; } = new();
}