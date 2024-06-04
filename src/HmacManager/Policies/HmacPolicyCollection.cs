using HmacManager.Common;
using HmacManager.Validation;

namespace HmacManager.Policies;

/// <summary>
/// A class representing a <c>HmacPolicyCollection</c>.
/// </summary>
public class HmacPolicyCollection : ComponentCollection<HmacPolicy>, IHmacPolicyCollection
{
    /// <summary>
    /// Validates an <c>HmacPolicy</c> before it is added to the collection.
    /// </summary>
    protected readonly IValidator<HmacPolicy> Validator;

    /// <summary>
    /// Creates an empty <c>HmacPolicyCollection</c>.
    /// </summary>
    public HmacPolicyCollection()
    {
        Validator = new HmacPolicyValidator(); 
    }

    /// <summary>
    /// Creates an <c>HmacPolicyCollection</c> from a specified policy collection.
    /// </summary>
    /// <param name="policies">An <c>IEnumerable</c> of <c>HmacPolicy</c> objects.</param>
    public HmacPolicyCollection(IEnumerable<HmacPolicy> policies) : this()
    {
        foreach (var policy in policies)
        {
            Add(policy);
        }
    }

    /// <inheritdoc/>
    public IReadOnlyCollection<HmacPolicy> Values => GetAll();

    /// <inheritdoc/>
    public void Add(HmacPolicy policy)
    {
        var validationResult = Validator.Validate(policy);
        if (validationResult.IsValid)
        {
            Add(policy.Name!, policy); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }

    /// <inheritdoc/>
    public new void Remove(string policy) => base.Remove(policy);
}