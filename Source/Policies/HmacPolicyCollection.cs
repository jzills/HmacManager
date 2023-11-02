// using HmacManagement.Mvc;

// namespace HmacManagement.Policies;

// public class HmacPolicyCollection : IHmacPolicyCollection
// {
//     protected IDictionary<string, HmacPolicy> Policies = 
//         new Dictionary<string, HmacPolicy>();

//     public void AddDefaultPolicy(Action<HmacPolicy> configurePolicy) =>
//         Add(HmacAuthenticationDefaults.DefaultPolicy, configurePolicy);

//     public void Add(string name, Action<HmacPolicy> configurePolicy)
//     {
//         var policy = new HmacPolicy(name);
//         configurePolicy.Invoke(policy);

//         Policies.Add(name, policy);
//     }

//     public HmacPolicy? Get(string name)
//     {
//         ArgumentNullException.ThrowIfNullOrEmpty(name, nameof(name));

//         if (Policies.ContainsKey(name))
//         {
//             return Policies[name];
//         }
//         else
//         {
//             return default;
//         }
//     }

//     public IReadOnlyCollection<HmacPolicy> GetAll() => 
//         Policies.Values.ToList().AsReadOnly();
// }