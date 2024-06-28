namespace HmacManager.Common;

/// <summary>
/// A class representing a generic <c>ComponentCollection</c>.
/// </summary>
public abstract class ComponentCollection<TComponent>
    : IComponentCollection<TComponent> 
        where TComponent : class
{
    /// <summary>
    /// A dictionary of components.
    /// </summary>
    protected readonly IDictionary<string, TComponent> Components = 
        new Dictionary<string, TComponent>();

    /// <summary>
    /// Adds a <c>TComponent</c> to the collection stored at the key name.
    /// </summary>
    /// <param name="name">A <c>string</c> representing the name of the component.</param>
    /// <param name="component">A <c>TComponent</c>.</param>
    protected void Add(string name, TComponent component)
    {
        if (!Components.ContainsKey(name))
        {
            Components.Add(name, component);
        }
    }

    /// <summary>
    /// Removes a <c>TComponent</c> from the collection stored at the key name.
    /// </summary>
    /// <param name="name">A <c>string</c> representing the name of the component.</param>
    protected void Remove(string name)
    {
        if (!string.IsNullOrWhiteSpace(name))
        {
            Components.Remove(name);
        }
    }

    /// <summary>
    /// Gets a <c>TComponent</c> from the collection stored at the key name.
    /// </summary>
    /// <param name="name">A <c>string</c> representing the name of the component.</param>
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

    /// <summary>
    /// Gets the entire <c>TComponent</c> collection.
    /// </summary>
    public IReadOnlyCollection<TComponent> GetAll() => 
        Components.Values.ToList().AsReadOnly();
}