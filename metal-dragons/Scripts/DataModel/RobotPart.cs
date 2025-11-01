using System.Collections.Generic;
using System.Text.Json.Serialization;

public class RobotPart
{
	public string Name { get; set; }
	public int Tier { get; set; }
	public int Level { get; set; }
	public PartFamily Family { get; set; }
	public int EquippedAttack { get; set; }
	public int EquippedHP { get; set; }
	public List<GameEffect> Effects { get; set; }
	
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public PartFamily Family2 { get; set; }
	[JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
	public PartFamily Family3 { get; set; }
}
