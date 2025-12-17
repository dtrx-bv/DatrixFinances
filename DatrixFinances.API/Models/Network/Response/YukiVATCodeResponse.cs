using Newtonsoft.Json;

namespace DatrixFinances.API.Models.Network.Response;

public class YukiVATCode
{
    public int Type { get; set; } = int.MinValue;
    public string Description { get; set; } = string.Empty;
    public string TypeDescription { get; set; } = string.Empty;
    public double Percentage { get; set; } = double.MinValue;
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public string? Country { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? StartDate { get; set; }
    [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
    public DateTimeOffset? EndDate { get; set; }
}