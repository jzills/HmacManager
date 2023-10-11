namespace HmacManagement.Components;

public interface IHmacManagerFactory
{
    IHmacManager Create(string policy);
    IHmacManager Create(string policy, string headerScheme);
}