namespace HmacManagement.Policies;

public interface IComponentCollection<TComponent>
{
    TComponent? Get(string name);
    IReadOnlyCollection<TComponent> GetAll();
}

public abstract class ComponentCollection<TComponent>
    : IComponentCollection<TComponent> 
        where TComponent : class
{
    protected readonly IDictionary<string, TComponent> Components = 
        new Dictionary<string, TComponent>();

    protected void Add(string name, TComponent component)
    {
        if (!Components.ContainsKey(name))
        {
            Components.Add(name, component);
        }
    }

    public TComponent? Get(string name)
    {
        if (!Components.ContainsKey(name))
        {
            return default;
        }
        else
        {
            return Components[name];
        }
    }

    public IReadOnlyCollection<TComponent> GetAll() => 
        Components.Values.ToList().AsReadOnly();
}

public interface IConfigurator<TComponent>
{
    void Add(string name, Action<TComponent> configureComponent);
}

public class HmacPolicyCollection
    : ComponentCollection<HmacPolicy>, IConfigurator<HmacPolicy>
{
    protected IValidator<HmacPolicy> Validator => new HmacPolicyValidator();

    public void Add(string name, Action<HmacPolicy> configurePolicy)
    {
        var policy = new HmacPolicy(name);
        configurePolicy.Invoke(policy);

        var validationResult = Validator.Validate(policy);
        if (validationResult.IsValid)
        {
            base.Add(name, policy); 
        }
        else
        {
            throw validationResult.GetError();
        }
    }
}