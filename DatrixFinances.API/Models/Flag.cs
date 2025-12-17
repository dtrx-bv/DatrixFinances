namespace DatrixFinances.API.Models;

[Flags]
public enum Flag
{
    None = 0,
    PotentialMalicious = 1 << 0, 
    Malicious = 1 << 1,    
    TrustedRequest = 1 << 2,     
    Unknown = 1 << 3             
}