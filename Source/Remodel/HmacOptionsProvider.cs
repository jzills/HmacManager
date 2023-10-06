namespace HmacManagement.Remodel;

public interface IHmacOptionsProvider
{
    HmacOptions? Get(string name);
}

public class HmacOptionsProvider : IHmacOptionsProvider
{
    protected IDictionary<string, HmacOptions> Options;
    internal HmacOptionsProvider(IDictionary<string, HmacOptions> options) 
        => Options = options;

    public HmacOptions? Get(string name)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

        if (Options.ContainsKey(name))
        {
            return Options[name];
        }
        else
        {
            return default;
        }
    }
}