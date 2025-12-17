using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace DatrixFinances.API.Models.DTO;

public class User
{
    [Key]
    public string Id { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
    public string Bearer { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool IsAuthorized { get; set; }
    public DateTime TokenExpiry { get; set; }
    public string YukiApiKey { get; set; } = string.Empty;
}