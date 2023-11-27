using HmacManagement.Common;
using HmacManagement.Policies;

namespace HmacManagement.Headers;

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