using HmacManager.Headers;

namespace HmacManager.Exceptions;

public class MissingHeaderException : Exception
{
    public MissingHeaderException() 
        : base("One or more headers defined in HmacManagerOptions is missing.")
    {
    }

    public static void ThrowIfMissing(
        string[] requiredHeaders, 
        Header[]? headersToSign
    )
    {
        if (headersToSign is not null)
        {
            var hasMissingHeaders = 
                requiredHeaders
                    .Except(headersToSign
                    .Select(header => header.Name))
                    .Any();

            if (hasMissingHeaders)
            {
                throw new MissingHeaderException();
            }
        }
    }
}