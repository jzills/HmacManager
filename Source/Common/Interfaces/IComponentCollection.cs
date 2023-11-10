namespace HmacManagement.Policies;

public interface IComponentCollection<TComponent>
{
    TComponent? Get(string name);
    IReadOnlyCollection<TComponent> GetAll();
}