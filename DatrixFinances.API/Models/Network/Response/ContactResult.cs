namespace DatrixFinances.API.Models.Network.Response;

public class ContactResult
{
    public DateOnly TimeStamp { get; set; } = DateOnly.MinValue;
    public string Succeeded { get; set; } = string.Empty;
    public string ContactId { get; set; } = string.Empty;
    public List<string> Failed { get; set; } = [];
}