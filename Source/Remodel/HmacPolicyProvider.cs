namespace HmacManagement.Remodel;

public interface IHmacPolicyProvider
{
    HmacPolicy? Get(string name);
}

public class HmacPolicyProvider : IHmacPolicyProvider
{
    protected IDictionary<string, HmacPolicy> Options;
    public HmacPolicyProvider(IDictionary<string, HmacPolicy> options) 
        => Options = options;

    public HmacPolicy? Get(string name)
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