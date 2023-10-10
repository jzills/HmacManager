using System.Text;
using HmacManagement.Caching;
using HmacManagement.Components;
using HmacManagement.Remodel;

public class DataSource_HmacPolicies
{
    public static IDictionary<string, HmacPolicy>[] GetPolicies() =>
        new Dictionary<string, HmacPolicy>[]
        {
            new Dictionary<string, HmacPolicy>
            {
                { "Default", 
                    new HmacPolicy 
                    {
                        Algorithms = new Algorithms
                        {
                            ContentAlgorithm = ContentHashAlgorithm.SHA1,
                            SigningAlgorithm = SigningHashAlgorithm.HMACSHA1
                        },
                        Keys = new KeyCredentials
                        {
                            PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("MyDefaultPrivateKey")),
                            PublicKey = Guid.NewGuid()
                        },
                        Nonce = new Nonce
                        {
                            MaxAge = TimeSpan.FromSeconds(30),
                            CacheType = NonceCacheType.Memory
                        }
                    }
                },
                { "AccountPolicy", 
                    new HmacPolicy 
                    {
                        Algorithms = new Algorithms
                        {
                            ContentAlgorithm = ContentHashAlgorithm.SHA1,
                            SigningAlgorithm = SigningHashAlgorithm.HMACSHA1
                        },
                        Keys = new KeyCredentials
                        {
                            PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("MyPrivateKey")),
                            PublicKey = Guid.NewGuid()
                        },
                        Nonce = new Nonce
                        {
                            MaxAge = TimeSpan.FromSeconds(30),
                            CacheType = NonceCacheType.Memory
                        }
                    }
                },
                { "UserPolicy", 
                    new HmacPolicy 
                    {
                        Algorithms = new Algorithms
                        {
                            ContentAlgorithm = ContentHashAlgorithm.SHA256,
                            SigningAlgorithm = SigningHashAlgorithm.HMACSHA256
                        },
                        Keys = new KeyCredentials
                        {
                            PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("123456789")),
                            PublicKey = Guid.NewGuid()
                        },
                        Nonce = new Nonce
                        {
                            MaxAge = TimeSpan.FromMinutes(1),
                            CacheType = NonceCacheType.Memory
                        }
                    }
                },
                { "EmailPolicy", 
                    new HmacPolicy 
                    {
                        Algorithms = new Algorithms
                        {
                            ContentAlgorithm = ContentHashAlgorithm.SHA512,
                            SigningAlgorithm = SigningHashAlgorithm.HMACSHA512
                        },
                        Keys = new KeyCredentials
                        {
                            PrivateKey = Convert.ToBase64String(Encoding.UTF8.GetBytes("!@#$%^&*()/\\/\\")),
                            PublicKey = Guid.NewGuid()
                        },
                        Nonce = new Nonce
                        {
                            MaxAge = TimeSpan.FromHours(1),
                            CacheType = NonceCacheType.Distributed
                        }
                    }
                }
            }
        };
}