using HmacManager.Common;
using HmacManager.Policies;

namespace HmacManager.Headers;

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
            Add(name, scheme); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}