using HmacManagement.Common;
using HmacManagement.Policies;

namespace HmacManagement.Headers;

public class HeaderCollection 
    : ComponentCollection<Header>, IConfigurator<Header>
{
    protected IValidator<Header> Validator = new HeaderValidator();
    
    public void Add(string name, Action<Header> configureHeader)
    {
        var header = new Header();
        configureHeader.Invoke(header);

        var validationResult = Validator.Validate(header);
        if (validationResult.IsValid)
        {
            base.Add(name, header); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}