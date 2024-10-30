using HmacManager.Components;

namespace HmacManager.Exceptions;

/// <summary>
/// A class representing an exception that occurs due to signing issues.
/// </summary>
public class HmacSigningException : Exception
{
    /// <summary>
    /// An instance of <c>HmacResult</c> representing the signing attempt.
    /// </summary> 
    public readonly HmacResult Result;

    /// <summary>
    /// Creates an instance of <c>HmacSigningException</c>.
    /// </summary>
    /// <param name="result">A <c>HmacResult</c> representing the signing attempt.</param> 
    /// <param name="message">An optional error message.</param>
    public HmacSigningException(HmacResult result, string? message = null) 
        : base(message ?? "The hmac signing result indicated an error.")
    {
        Result = result;
    }
}