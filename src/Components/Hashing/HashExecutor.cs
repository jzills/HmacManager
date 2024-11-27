using System.Security.Cryptography;
using System.Text;

namespace HmacManager.Components;

/// <summary>
/// Provides functionality to compute a hash for a given content using a specified <see cref="HashAlgorithm"/>.
/// </summary>
internal class HashExecutor
{
    /// <summary>
    /// Gets the <see cref="HashAlgorithm"/> used for hashing operations.
    /// </summary>
    protected readonly HashAlgorithm Algorithm;

    /// <summary>
    /// Initializes a new instance of the <see cref="HashExecutor"/> class with a specified hashing algorithm.
    /// </summary>
    /// <param name="algorithm">The <see cref="HashAlgorithm"/> to be used for hashing.</param>
    public HashExecutor(HashAlgorithm algorithm) => Algorithm = algorithm;

    /// <summary>
    /// Computes the hash of the provided content asynchronously and returns the result as a Base64-encoded string.
    /// </summary>
    /// <param name="content">The input string to hash.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the Base64-encoded hash string.</returns>
    public async Task<string> ExecuteAsync(string content)
    {
        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
        var hashBytes = await Algorithm.ComputeHashAsync(stream);
        return Convert.ToBase64String(hashBytes);
    }
}