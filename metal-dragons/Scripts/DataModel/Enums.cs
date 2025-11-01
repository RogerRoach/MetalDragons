using System.Text.Json.Serialization;

public enum PartFamily
{
	None,
	Biomech,
	Demon,
	Eldritch,
	Elemental,
	Magitech,
	Magnetic,
	Nanotech,
	Pirated,
	Plasma,
	Steampunk,
	All
}

public enum Trigger
{
	AllyBuffed,
	AllyDebuffed,
	AllyKnockedDown,
	AfterSummonAlly,
	AfterSummonEnemy,
	Attacked,
	Attacking,
	BattleEnd,
	BattleStart,
	BeforeAttack,
	BeforeCombatAction,
	Buff,
	Debuff,
	EnterStore,
	Equipped,
	EveryBeat,
	Every2Beats,
	Every3Beats,
	Every4Beats,
	ExitStore,
	DragonAddedToBench,
	DragonPartAddedToBench,
	DragonPartPurchased,
	DragonPartSold,
	DragonEntersPlay,
	DragonPartEntersPlay,
	DragonPurchased,
	DragonSold,
	KnockedDown,
	SelfBuffed,
	Sold,
	SpellAddedToBench,
	SpellCast,
	SteamChargeUsedOnAlly,
	SummonCompleted,
	SummonStarted,
	Targeted,
	TeamEquipsNanopart
}

public enum Target
{
	Allies,
	AllyBackRow,
	AllyFrontRow,
	Enemies,
	EnemyBackRow,
	EnemyFrontRow,
	Hand,
	LeftAlly,
	RandomAlly,
	RandomEnemy,
	RandomShopPart,
	RandomUnit,
	RightAlly,
	Self,
	Target,
	TriggeringUnit
}

public enum SummonTarget
{
	RandomAllyBackTile,
	RandomAllyFrontTile,
	RandomAllyMidTile,
	RandomAllyTile,
	RandomEnemyBackTile,
	RandomEnemyFrontTile,
	RandomEnemyMidTile,
	RandomEnemyTile
}

public enum EffectType
{
	ApplySteamCharge,
	ApplyShield,
	BuffAll,
	BuffAttack,
	BuffHealth,
	Consume,
	DebuffAll,
	DebuffAttack,
	DebuffHealth,
	DisablePart,
	GenerateSteamCharge,
	GainGold
}
