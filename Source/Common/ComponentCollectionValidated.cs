using HmacManagement.Policies;

namespace HmacManagement.Common;

public class ComponentCollectionValidated<TComponent, TValidator> 
    : ComponentCollection<TComponent> 
        where TComponent : class
        where TValidator : IValidator<TComponent>, new()
{
    protected IValidator<TComponent> Validator => new TValidator();

    public void Add(string name, Action<TComponent> configureComponent)
    {
        var component = (TComponent?)Activator.CreateInstance(typeof(TComponent), args: name);
        if (component is null)
        {
            throw new ArgumentException($"An error occurred creating an instance of type \"{typeof(TComponent)}\" with name \"{name}\".");
        }
        else
        {
            configureComponent.Invoke(component);

            var validationResult = Validator.Validate(component);
            if (validationResult.IsValid)
            {
                base.Add(name, component); 
            }
            else
            {
                throw validationResult.GetError();
            }
        }
    }
}