using System.Text.Json;
using System.Text.Json.Serialization;

namespace NKF.Domain.SeedWorks;

public sealed class EntityIdJsonConverter<TId> : JsonConverter<TId> where TId : EntityId
{
    public override TId? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string for {typeToConvert.Name} but got {reader.TokenType}.");
        }

        string? value = reader.GetString();
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (!Guid.TryParse(value, out Guid guid))
        {
            throw new JsonException($"Invalid GUID value '{value}' for {typeToConvert.Name}.");
        }

        System.Reflection.MethodInfo? createMethod = typeToConvert.GetMethod(
            "Create",
            System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
            null,
            new Type[] { typeof(Guid) },
            null);

        if (createMethod == null)
        {
            createMethod = typeToConvert.GetMethod(
                "Create",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static,
                null,
                new Type[] { typeof(Guid?) },
                null);
        }

        if (createMethod == null)
        {
            throw new JsonException($"No Create method found for {typeToConvert.Name}.");
        }

        object? result = createMethod.Invoke(null, new object?[] { guid });
        if (result is FluentResults.Result<TId> typedResult)
        {
            if (typedResult.IsFailed)
            {
                throw new JsonException($"Failed to create {typeToConvert.Name} from '{value}'.");
            }

            return typedResult.Value;
        }

        throw new JsonException($"Unexpected result type from {typeToConvert.Name}.Create.");
    }

    public override void Write(Utf8JsonWriter writer, TId value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.Value.ToString());
    }
}
