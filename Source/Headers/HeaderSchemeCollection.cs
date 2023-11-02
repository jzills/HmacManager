using HmacManagement.Policies;

namespace HmacManagement.Headers;

// public class HeaderSchemeCollection
// {
//     protected IDictionary<string, HeaderScheme> Schemes = new Dictionary<string, HeaderScheme>();
    
//     public IReadOnlyDictionary<string, HeaderScheme> GetHeaderSchemes() => Schemes.AsReadOnly();

//     public HeaderScheme? GetHeaderScheme(string name)
//     {
//         if (Schemes.ContainsKey(name))
//         {
//             return Schemes[name];
//         }
//         else
//         {
//             return default;
//         }
//     }

//     public void AddHeaderScheme(string name, Action<HeaderScheme> configureScheme)
//     {
//         ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

//         var scheme = new HeaderScheme(name);
//         configureScheme.Invoke(scheme);

//         ArgumentNullException.ThrowIfNull(scheme);

//         Schemes.Add(scheme.Name, scheme);
//     }
// }

public class HeaderSchemeValidator : IValidator<HeaderScheme>
{
    public ValidationResult Validate(HeaderScheme validatable) => new ValidationResult(isValid: true);
}

public class HeaderSchemeCollection 
    : ComponentCollection<HeaderScheme>, IConfigurator<HeaderScheme>
{
    protected IValidator<HeaderScheme> Validator => new HeaderSchemeValidator();

    public void Add(string name, Action<HeaderScheme> configureScheme)
    {
        var scheme = new HeaderScheme(name);
        configureScheme.Invoke(scheme);

        var validationResult = Validator.Validate(scheme);
        if (validationResult.IsValid)
        {
            base.Add(name, scheme); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}