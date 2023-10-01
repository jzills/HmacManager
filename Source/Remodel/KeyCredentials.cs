namespace HmacManagement.Remodel;

public class KeyCredentials
{
    public Guid PublicKey 
    { 
        get => PublicKey; 
        set
        {
            if (value == Guid.Empty)
            {
                throw new FormatException("The value for \"PublicKey\" cannot be an empty guid.");
            }
            else
            {
                PublicKey = value;
            }
        }
    }

    public string PrivateKey 
    { 
        get => PrivateKey; 
        set
        {
            var buffer = new Span<byte>(new byte[value.Length]);
            if (Convert.TryFromBase64String(value, buffer, out _))
            {
                PrivateKey = value;
            }
            else
            {
                throw new FormatException("The supplied \"PrivateKey\" is not a valid Base64 encoded string.");
            }
        }
    }
}