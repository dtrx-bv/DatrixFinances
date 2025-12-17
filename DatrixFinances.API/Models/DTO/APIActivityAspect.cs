using System.ComponentModel.DataAnnotations;

namespace DatrixFinances.API.Models.DTO;

public class APIActivityAspect
{
    [Key]
    public long Id { get; set; }
    public DateTimeOffset DateTimeOffset { get; set; } = new();
    public string IPv4 { get; set; } = string.Empty;
    public string IPv6 { get; set; } = string.Empty;
    public string OriginPlatform { get; set; } = string.Empty;
    public string OriginCompany { get; set; } = string.Empty;
    public string SignedInCompanyUser { get; set; } = string.Empty;
    public int StatusCode { get; set; } = -1;
    public string StatusCodeName { get; set; } = string.Empty;
    public string HttpMethod { get; set; } = string.Empty;
    public string ControllerMethod { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public double ElapsedMilliseconds { get; set; } = -1;
    public string Flag { get; set; } = ((Flag)0).ToString();
    public string SentPayload { get; set; } = string.Empty;
    public string SentPayloadContentType { get; set; } = string.Empty;
    public string ReceivedPayload { get; set; } = string.Empty;
    public string ReceivedPayloadContentType { get; set; } = string.Empty;
}