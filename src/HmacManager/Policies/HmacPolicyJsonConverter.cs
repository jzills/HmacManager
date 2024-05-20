using System.Text.Json;
using System.Text.Json.Serialization;

namespace HmacManager.Policies;

public class HmacPolicyJsonConverter : JsonConverter<HmacPolicy>
{
    public override HmacPolicy? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var builder = new HmacPolicyBuilderJsonReader();
        while (reader.Read())
        {
            if (reader.TokenType is JsonTokenType.PropertyName)
            {
                var propertyName = reader.GetString();
                reader.Read();
                
                switch (propertyName)
                {
                    case nameof(HmacPolicy.Name):
                        builder.UseName(ref reader);
                        break;
                    case nameof(HmacPolicy.Keys.PublicKey):
                        builder.UsePublicKey(ref reader);
                        break;
                    case nameof(HmacPolicy.Keys.PrivateKey):
                        builder.UsePrivateKey(ref reader);
                        break;
                    case nameof(HmacPolicy.Algorithms.ContentHashAlgorithm):
                        builder.UseContentHashAlgorithm(ref reader);
                        break;
                    case nameof(HmacPolicy.Algorithms.SigningHashAlgorithm):
                        builder.UseSigningHashAlgorithm(ref reader);
                        break;
                    case nameof(HmacPolicy.Nonce.CacheType):
                        builder.UseCacheType(ref reader);
                        break;
                    case nameof(HmacPolicy.Nonce.MaxAge):
                        builder.UseMaxAge(ref reader);
                        break;
                    case nameof(HmacPolicy.HeaderSchemes):
                        builder.AddSchemes(ref reader);
                        break;
                }
            }
        }

        return builder.Build();
    }

    public override void Write(Utf8JsonWriter writer, HmacPolicy value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Name", value.Name);

        // Keys
        writer.WritePropertyName("Keys");
        writer.WriteStartObject();
        writer.WriteString("PublicKey", value.Keys.PublicKey);
        writer.WriteString("PrivateKey", value.Keys.PrivateKey);
        writer.WriteEndObject();

        // Algorithms
        writer.WritePropertyName("Algorithms");
        writer.WriteStartObject();
        writer.WriteString("ContentHashAlgorithm", Enum.GetName(value.Algorithms.ContentHashAlgorithm));
        writer.WriteString("SigningHashAlgorithm", Enum.GetName(value.Algorithms.SigningHashAlgorithm));
        writer.WriteEndObject();

        // Nonce
        writer.WritePropertyName("Nonce");
        writer.WriteStartObject();
        writer.WriteString("CacheType", Enum.GetName(value.Nonce.CacheType));
        writer.WriteNumber("MaxAge", value.Nonce.MaxAge.TotalSeconds);
        writer.WriteEndObject();

        // Header Schemes
        writer.WritePropertyName("HeaderSchemes");
        writer.WriteStartArray();

        foreach (var headerScheme in value.HeaderSchemes.GetAll())
        {
            writer.WriteStartObject();
            writer.WriteString("Name", headerScheme.Name);
            writer.WritePropertyName("Headers");
            writer.WriteStartArray();
            foreach (var header in headerScheme.Headers)
            {
                writer.WriteStartObject();
                writer.WriteString("Name", header.Name);
                writer.WriteString("ClaimType", header.ClaimType);
                writer.WriteEndObject();
            }
            
            writer.WriteEndArray();
            writer.WriteEndObject();
        }

        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}