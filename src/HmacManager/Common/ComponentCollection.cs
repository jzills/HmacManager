namespace HmacManager.Common;

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