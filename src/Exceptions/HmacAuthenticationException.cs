namespace HmacManager.Exceptions;

internal class HmacAuthenticationException : Exception
{
    public HmacAuthenticationException() 
        : base("Hmac authentication failed with the current manager configuration.")
    {
    }
}