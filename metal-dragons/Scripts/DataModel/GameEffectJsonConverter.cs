using System;
using System.Text.Json;
using System.Text.Json.Serialization;

public class GameEffectJsonConverter : JsonConverter<GameEffect>
{
	public override GameEffect Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		using var doc = JsonDocument.ParseValue(ref reader);
		var root = doc.RootElement;

		var effectType = root.GetProperty("TypeName").GetString();
		return effectType switch
		{
			"SingleTargetGameEffect" => JsonSerializer.Deserialize<SingleTargetGameEffect>(root.GetRawText(), options),
			"SummonGameEffect" => JsonSerializer.Deserialize<SummonGameEffect>(root.GetRawText(), options),
			_ => throw new NotSupportedException($"Unknown GameEffect type: {effectType}")
		};
	}

	public override void Write(Utf8JsonWriter writer, GameEffect value, JsonSerializerOptions options)
	{
		var typeName = value.GetType().Name;
		var json = JsonSerializer.SerializeToElement(value, value.GetType(), options);
		
		writer.WriteStartObject();
		writer.WriteString("TypeName", typeName);
		foreach (var prop in json.EnumerateObject())
		{
			prop.WriteTo(writer);
		}
		writer.WriteEndObject();
	}
}
