namespace HmacManager.Policies;

internal class KeyCredentials
{
    public Guid PublicKey { get; set; } = Guid.Empty;
    public string PrivateKey { get; set; } = string.Empty;
}