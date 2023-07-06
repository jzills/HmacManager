using Source.Components;

namespace Source.Exceptions;

public class MissingHeaderException : Exception
{
    public MissingHeaderException() 
        : base("One or more headers defined in HMACManagerOptions is missing.")
    {
    }

    public static void ThrowIfMissing(
        string[] requiredHeaders, 
        MessageContent[]? messageContent
    )
    {
        if (messageContent is not null)
        {
            var hasMissingHeaders = 
                requiredHeaders
                    .Except(messageContent
                    .Select(content => content.Header))
                    .Any();

            if (hasMissingHeaders)
            {
                throw new MissingHeaderException();
            }
        }
    }
}