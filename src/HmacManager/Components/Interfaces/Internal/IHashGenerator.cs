namespace HmacManager.Components;

internal interface IHashGenerator
{
    Task<string> HashAsync(string content);
}