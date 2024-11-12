using System.Text.Json;
using System.Text.Json.Serialization;
using HmacManager.Extensions;

namespace HmacManager.Headers;

/// <summary>
/// A custom JSON converter for serializing and deserializing <see cref="HeaderSchemeCollection"/>.
/// </summary>
public class HeaderSchemeCollectionJsonConverter : JsonConverter<HeaderSchemeCollection>
{
    /// <summary>
    /// Reads and deserializes a <see cref="HeaderSchemeCollection"/> from the provided JSON.
    /// </summary>
    /// <param name="reader">The <see cref="Utf8JsonReader"/> to read from.</param>
    /// <param name="typeToConvert">The target type to convert to.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> used during deserialization.</param>
    /// <returns>The deserialized <see cref="HeaderSchemeCollection"/>.</returns>
    public override HeaderSchemeCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var headerSchemeCollection = new HeaderSchemeCollection();
        if (reader.TokenType is JsonTokenType.StartArray)
        {
            string schemeName = default!;
            List<Header> headers = new();
            while (reader.Read() && reader.TokenType is not JsonTokenType.EndArray)
            {
                if (reader.TokenType is JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();
                    
                    if (propertyName == nameof(HeaderScheme.Name) && 
                        !reader.TryGetString(out schemeName))
                    {
                        // Error
                    } 
                    else if (propertyName == nameof(HeaderScheme.Headers) &&
                        !reader.TryGetHeaders(out headers))
                    {
                        // Error
                    }
                }

                if (reader.TokenType is JsonTokenType.EndObject)
                {
                    var scheme = new HeaderScheme(schemeName);
                    foreach (var header in headers)
                    {
                        scheme.AddHeader(header.Name, header.ClaimType);
                    }
                    
                    headerSchemeCollection.Add(scheme);
                }
            }
        }
        else
        {
            // Error
        }

        return headerSchemeCollection;
    }

    /// <summary>
    /// Writes a <see cref="HeaderSchemeCollection"/> to JSON.
    /// </summary>
    /// <param name="writer">The <see cref="Utf8JsonWriter"/> to write the JSON to.</param>
    /// <param name="value">The <see cref="HeaderSchemeCollection"/> to serialize.</param>
    /// <param name="options">The <see cref="JsonSerializerOptions"/> used during serialization.</param>
    public override void Write(Utf8JsonWriter writer, HeaderSchemeCollection value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var headerScheme in value.GetAll())
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
    }
}