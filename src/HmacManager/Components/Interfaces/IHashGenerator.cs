namespace HmacManager.Components;

public interface IHashGenerator
{
    Task<string> HashAsync(string content);
}