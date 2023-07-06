namespace Souce.Mvc;

public static class HMACAuthenticationDefaults
{
    public const string Scheme = "HMAC";
    public class Headers
    {
        public const string Nonce = "X-Nonce";
        public const string RequestedOn = "X-RequestedOn";
    }
}