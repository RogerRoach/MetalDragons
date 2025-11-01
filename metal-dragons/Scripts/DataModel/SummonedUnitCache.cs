using Godot;
using System;
using System.IO;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public static class SummonedUnitCache
{
	private static readonly JsonSerializerOptions _options = new()
	{
		WriteIndented = true,
		Converters = { new GameEffectJsonConverter(),  new JsonStringEnumConverter() }
	};
	
	private static string _path = "res://Data/units.json";
	
	public static Dictionary<string, SummonedUnit> Load()
	{
		try
		{
			if (!File.Exists(ProjectSettings.GlobalizePath(_path)))
			{
				GD.Print($"No summoned units file found at {_path}. Creating a new one.");
				Save(new());
				return new();
			}

			var json = File.ReadAllText(ProjectSettings.GlobalizePath(_path));
			return JsonSerializer.Deserialize<Dictionary<string, SummonedUnit>>(json, _options) ?? new();
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to load summoned units: {ex}");
			return new();
		}
	}
	
	public static void Save(Dictionary<string, SummonedUnit> parts)
	{
		try
		{
			var json = JsonSerializer.Serialize(parts, _options);
			File.WriteAllText(ProjectSettings.GlobalizePath(_path), json);
			GD.Print($"Saved summoned units to {_path}");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to save summoned units: {ex}");
		}
	}
}
