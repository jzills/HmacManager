using HmacManager.Components;

namespace HmacManager.Exceptions;

internal class HmacSigningException : Exception
{
    public readonly HmacResult SigningResult;
    public readonly HttpRequestMessage Request;

    public HmacSigningException(HmacResult signingResult, HttpRequestMessage request) 
        : base("The hmac signing result indicated an error.")
    {
        SigningResult = signingResult;
        Request = request;
    }
}