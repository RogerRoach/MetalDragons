using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using Godot;

public static class RobotPartCache
{
	private static readonly JsonSerializerOptions _options = new()
	{
		WriteIndented = true,
		Converters = { new GameEffectJsonConverter(),  new JsonStringEnumConverter() }
	};

	private static string _path = "res://Data/robot_parts.json";

	public static List<RobotPart> Load()
	{
		try
		{
			if (!File.Exists(ProjectSettings.GlobalizePath(_path)))
			{
				GD.Print($"No robot parts file found at {_path}. Creating a new one.");
				Save(new());
				return new();
			}

			var json = File.ReadAllText(ProjectSettings.GlobalizePath(_path));
			return JsonSerializer.Deserialize<List<RobotPart>>(json, _options) ?? new();
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to load robot parts: {ex}");
			return new();
		}
	}

	public static void Save(List<RobotPart> parts)
	{
		try
		{
			var json = JsonSerializer.Serialize(parts, _options);
			File.WriteAllText(ProjectSettings.GlobalizePath(_path), json);
			GD.Print($"Saved robot parts to {_path}");
		}
		catch (Exception ex)
		{
			GD.PrintErr($"Failed to save robot parts: {ex}");
		}
	}
}
