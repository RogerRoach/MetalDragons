public abstract class GameEffect
{
	public Trigger Trigger { get; set; }
}

public class SingleTargetGameEffect : GameEffect
{
	public Target Target { get; set; }
	public Effect Effect { get; set; }
}

public record Effect(EffectType Type, int Magnitude);

public class SummonGameEffect : GameEffect
{
	public string UnitToSummon { get; set; } = "";
	public SummonTarget Target { get; set; }
	public int SummonCount { get; set; } = 1;
}
