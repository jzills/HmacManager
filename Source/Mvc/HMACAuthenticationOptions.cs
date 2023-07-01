using Microsoft.AspNetCore.Authentication;

namespace Source.Mvc;

public class HMACAuthenticationOptions : AuthenticationSchemeOptions
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public TimeSpan MaxAge { get; set; }
    public string[] AdditionalContentHeaders { get; set; } 
}