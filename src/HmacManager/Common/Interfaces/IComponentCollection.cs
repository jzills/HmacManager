namespace HmacManager.Common;

public interface IComponentCollection<TComponent>
{
    TComponent? Get(string name);
    IReadOnlyCollection<TComponent> GetAll();
}