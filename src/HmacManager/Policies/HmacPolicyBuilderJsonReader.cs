using System.Text.Json;
using HmacManager.Caching;
using HmacManager.Components;
using HmacManager.Headers;

namespace HmacManager.Policies;

public class HmacPolicyBuilderJsonReader : HmacPolicyBuilder
{
    public string? Name { get; set; }
    public NonceCacheType CacheType { get; set; }
    public TimeSpan MaxAgeInSeconds { get; set; }

    public HmacPolicyBuilderJsonReader UseName(ref Utf8JsonReader reader)
    {
        var value = reader.GetString();
        Name = value;

        return this;
    }

    public HmacPolicyBuilderJsonReader UsePublicKey(ref Utf8JsonReader reader)
    {
        var value = reader.GetGuid();
        UsePublicKey(value);

        return this;
    }

    public HmacPolicyBuilderJsonReader UsePrivateKey(ref Utf8JsonReader reader)
    {
        var value = reader.GetString();
        UsePrivateKey(value);

        return this;
    }

    public HmacPolicyBuilderJsonReader UseContentHashAlgorithm(ref Utf8JsonReader reader)
    {
        var value = reader.GetString();
        var @enum = Enum.Parse<ContentHashAlgorithm>(value);
        UseContentHashAlgorithm(@enum);

        return this;
    }

    public HmacPolicyBuilderJsonReader UseSigningHashAlgorithm(ref Utf8JsonReader reader)
    {
        var value = reader.GetString();
        var @enum = Enum.Parse<SigningHashAlgorithm>(value);
        UseSigningHashAlgorithm(@enum);

        return this;
    }

    public HmacPolicyBuilderJsonReader UseCacheType(ref Utf8JsonReader reader)
    {
        var value = reader.GetString();
        Nonce.CacheType = Enum.Parse<NonceCacheType>(value);
        return this;
    }

    public HmacPolicyBuilderJsonReader UseMaxAge(ref Utf8JsonReader reader)
    {
        var value = reader.GetDouble();
        Nonce.MaxAge = TimeSpan.FromSeconds(value);

        return this;
    }

    public HmacPolicyBuilder AddSchemes(ref Utf8JsonReader reader)
    {
        // Sanity check
        if (reader.TokenType is JsonTokenType.StartArray)
        {
            while (reader.Read())
            {
                if (reader.TokenType is JsonTokenType.StartObject)
                {
                    var schemeName = string.Empty;
                    var headers = new List<Header>();
                    while (reader.Read() && reader.TokenType is not JsonTokenType.EndObject)
                    {
                        if (reader.TokenType is JsonTokenType.PropertyName)
                        {
                            var propertyName = reader.GetString();
                            reader.Read();
                            
                            switch (propertyName)
                            {
                                case nameof(HeaderScheme.Name):
                                    schemeName = reader.GetString();
                                    break;
                                case nameof(HeaderScheme.Headers):
                                    while (reader.TokenType is not JsonTokenType.EndArray)
                                    {
                                        var header = new Header();
                                        while (reader.TokenType is not JsonTokenType.EndObject)
                                        {
                                            reader.Read();
                                            if (reader.TokenType is JsonTokenType.PropertyName)
                                            {
                                                var pname = reader.GetString();
                                                reader.Read();

                                                switch (pname)
                                                {
                                                    case "Name":
                                                        header.Name = reader.GetString();
                                                        break;
                                                    case "ClaimType":
                                                        header.ClaimType = reader.GetString();
                                                        break;
                                                }
                                            }
                                        }

                                        headers.Add(header);
                                        reader.Read(); // Read end object
                                    }
                                    break;
                            }
                        }
                    }

                    var headerScheme = new HeaderScheme(schemeName);
                    AddScheme(schemeName, _ => 
                    {
                        foreach (var header in headers)
                        {
                            _.AddHeader(header.Name, header.ClaimType);
                        }
                    });
                }
            }
        }

        return this;
    }

    public HmacPolicy Build() => Build(Name);
}