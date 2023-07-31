namespace HmacManagement.Mvc;

public static class HmacAuthenticationDefaults
{
    public const string Scheme = "Hmac";
    public class Headers
    {
        public const string Nonce = "X-Nonce";
        public const string RequestedOn = "X-RequestedOn";
    }
}
