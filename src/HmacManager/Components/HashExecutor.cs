using System.Security.Cryptography;
using System.Text;

namespace HmacManager.Components;

public class HashExecutor
{
    protected readonly HashAlgorithm Algorithm;

    public HashExecutor(HashAlgorithm algorithm) => Algorithm = algorithm;

    public async Task<string> ExecuteAsync(string content)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var hashBytes = await Algorithm.ComputeHashAsync(stream);
        return Convert.ToBase64String(hashBytes);
    }
}