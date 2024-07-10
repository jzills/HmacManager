namespace HmacManager.Components;

public interface ISigningContentBuilder
{
    SigningContentBuilder CreateBuilder();
    string Build();
}