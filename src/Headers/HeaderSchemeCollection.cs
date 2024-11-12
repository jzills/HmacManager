using System.Text.Json.Serialization;
using HmacManager.Common;
using HmacManager.Policies;

namespace HmacManager.Headers;

/// <summary>
/// A class representing a <c>HeaderSchemeCollection</c>.
/// </summary>
[JsonConverter(typeof(HeaderSchemeCollectionJsonConverter))]
public class HeaderSchemeCollection : ComponentCollection<HeaderScheme>
{
    /// <summary>
    /// An instance of a header scheme validator.
    /// </summary>
    protected IValidator<HeaderScheme> Validator => new HeaderSchemeValidator();

    /// <summary>
    /// Adds a <see cref="HeaderScheme"/> to the collection after validating it.
    /// </summary>
    /// <param name="scheme">The <see cref="HeaderScheme"/> to add to the collection.</param>
    /// <exception cref="Exception">Thrown if the <paramref name="scheme"/> is invalid.</exception>
    public void Add(HeaderScheme scheme)
    {
        var validationResult = Validator.Validate(scheme);
        if (validationResult.IsValid)
        {
            Add(scheme.Name, scheme); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}