namespace HmacManager.Common;

public interface IConfigurator<TComponent>
{
    void Add(string name, Action<TComponent> configureComponent);
}