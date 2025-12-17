namespace DatrixFinances.API.Models.Network.Response;

public class GlAccount
{
    public string Code { get; set; } = string.Empty;
    public int Type { get; set; } = int.MinValue;
    public int Subtype { get; set; } = int.MinValue;
    public bool IsEnabled { get; set; }
    public string Descripton { get; set; } = string.Empty;
    public string DescriptionNL { get; set; } = string.Empty;
    public string DescriptionFR { get; set; } = string.Empty;
    public string DescriptionEN { get; set; } = string.Empty;
    public bool IsVATApplicable { get; set; }
    public double DeductableVATPercentage { get; set; } = double.MinValue;
    public double ProfessionalPercentage { get; set; } = double.MinValue;
}