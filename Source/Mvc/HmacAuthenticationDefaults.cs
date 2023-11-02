namespace HmacManagement.Mvc;

public static class HmacAuthenticationDefaults
{
    public const string AuthenticationScheme = "Hmac";
    public const string DefaultPolicy = "Default";
    public class Headers
    {
        public const string Nonce = "X-Nonce";
        public const string DateRequested = "X-DateRequested";
    }
}
