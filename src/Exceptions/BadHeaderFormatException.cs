namespace HmacManager.Exceptions;

/// <summary>
/// A class representing an exception that occurs due to header formatting issues.
/// </summary>
public class BadHeaderFormatException : FormatException
{
    /// <summary>
    /// Creates an instance of <c>BadHeaderFormatException</c>.
    /// </summary>
    /// <param name="message">An optional error message.</param>
    public BadHeaderFormatException(string? message = null) 
        : base(message ?? "An expected header was found but the format is incorrect.")
    {
    }
}