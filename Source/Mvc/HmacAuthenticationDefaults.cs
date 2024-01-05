namespace HmacManager.Mvc;

public static class HmacAuthenticationDefaults
{
    public const string AuthenticationScheme = "Hmac";
    public const string DefaultPolicy = "Default";
    public class Headers
    {
        public const string Policy = "X-Hmac-Policy";
        public const string Scheme = "X-Hmac-Scheme";
        public const string Nonce = "X-Nonce";
        public const string DateRequested = "X-DateRequested";
    }
}
