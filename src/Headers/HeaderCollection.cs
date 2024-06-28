using HmacManager.Common;
using HmacManager.Policies;

namespace HmacManager.Headers;

public class HeaderCollection : ComponentCollection<Header>
{
    protected IValidator<Header> Validator = new HeaderValidator();
    
    public void Add(Header header)
    {
        var validationResult = Validator.Validate(header);
        if (validationResult.IsValid)
        {
            Add(header.Name, header); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}