namespace HmacManagement.Remodel;

public class KeyCredentials
{
    // public KeyCredentials(Guid publicKey, string privateKey)
    // {
    //     if (publicKey == Guid.Empty)
    //     {
    //         throw new FormatException("The value for \"PublicKey\" cannot be an empty guid.");
    //     }
    //     else
    //     {
    //         PublicKey = publicKey;
    //     }

    //     var buffer = new Span<byte>(new byte[privateKey.Length]);
    //     if (!Convert.TryFromBase64String(privateKey, buffer, out _))
    //     {
    //         throw new FormatException("The supplied \"PrivateKey\" is not a valid Base64 encoded string.");
    //     }
    //     else
    //     {
    //         PrivateKey = privateKey;
    //     }
    // }

    public Guid PublicKey { get; set; }
    public string PrivateKey { get; set; }
}