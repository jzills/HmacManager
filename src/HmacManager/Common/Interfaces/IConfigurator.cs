namespace HmacManager.Common;

internal interface IConfigurator<TComponent>
{
    void Add(string name, Action<TComponent> configureComponent);
}