namespace HmacManagement.Policies;

public class KeyCredentials
{
    public Guid PublicKey { get; set; } = Guid.Empty;
    public string PrivateKey { get; set; } = string.Empty;
}