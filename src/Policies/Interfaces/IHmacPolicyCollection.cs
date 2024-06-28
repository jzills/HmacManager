namespace HmacManager.Policies;

/// <summary>
/// An interface representing an <c>IHmacPolicyCollection</c>.
/// </summary>
public interface IHmacPolicyCollection
{
    /// <summary>
    /// Contains the entire collection of <c>HmacPolicy</c> objects.
    /// </summary>
    IReadOnlyCollection<HmacPolicy> Values { get; }

    /// <summary>
    /// Adds a specified <c>HmacPolicy</c> to the <c>HmacPolicyCollection</c>.
    /// </summary>
    /// <param name="policy">An <c>HmacPolicy</c>.</param>
    void Add(HmacPolicy policy);

    /// <summary>
    /// Removes a specified <c>HmacPolicy</c> from the <c>HmacPolicyCollection</c>.
    /// </summary>
    /// <param name="policy">The name of the policy to remove.</param>
    void Remove(string policy);
}