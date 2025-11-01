using Godot;
using System;
using System.Linq;

public partial class EffectEditor : HBoxContainer
{
	public event Action Changed;
	private GameEffect _effect;
	private OptionButton _triggerBox, _targetBox, _typeBox, _unitType;
	//private LineEdit _unitType; // TODO: Make this into an OptionButton with the list coming from the list of dragons you can summon.
	private SpinBox _magnitudeBox;

	public EffectEditor(GameEffect effect)
	{
		_effect = effect;
		if (effect is SingleTargetGameEffect stge)
		{
			InitSingleTargetGameEffectEditor(stge);
		}
		else if (effect is SummonGameEffect sge)
		{
			InitSummonGameEffectEditor(sge);
		}
	}

	public void InitSummonGameEffectEditor(SummonGameEffect effect)
	{
		_triggerBox = CreateEnumDropdown<Trigger>(effect.Trigger, val => effect.Trigger = val);
		_targetBox = CreateEnumDropdown<SummonTarget>(effect.Target, val => effect.Target = val);
		_unitType = new OptionButton();
		_magnitudeBox = new SpinBox { MinValue = 1, MaxValue = 9, Value = effect.SummonCount };
		
		_magnitudeBox.ValueChanged += v => {
			effect.SummonCount = (int)v ;
			Changed?.Invoke();
		};
		
		var unitsToSummon = SummonedUnitCache.Load().Keys.ToList();
		foreach (var unit in unitsToSummon)
		{
			_unitType.AddItem(unit);
		}
		
		var index = unitsToSummon.IndexOf(effect.UnitToSummon);
		
		if (index >= 0)
		{
			_unitType.Select(index);
		}
		
		_unitType.ItemSelected += t => {
			effect.UnitToSummon = unitsToSummon[(int)t];
			Changed?.Invoke();
		};
		
		AddChild(new Label { Text = "On" }); AddChild(_triggerBox);
		AddChild(new Label { Text = "Summon" }); AddChild(_magnitudeBox); AddChild(_unitType);
		AddChild(new Label { Text = "To" }); AddChild(_targetBox);
	}

	public void InitSingleTargetGameEffectEditor(SingleTargetGameEffect effect)
	{
		_triggerBox = CreateEnumDropdown<Trigger>(effect.Trigger, val => effect.Trigger = val);
		_targetBox = CreateEnumDropdown<Target>(effect.Target, val => effect.Target = val);
		_typeBox = CreateEnumDropdown<EffectType>(effect.Effect.Type, val => effect.Effect = effect.Effect with { Type = val });
		_magnitudeBox = new SpinBox { MinValue = 1, MaxValue = 999, Value = effect.Effect.Magnitude };

		_magnitudeBox.ValueChanged += v => {
			effect.Effect = effect.Effect with { Magnitude = (int)v };
			Changed?.Invoke();
		};

		AddChild(new Label { Text = "On" }); AddChild(_triggerBox);
		AddChild(_typeBox);
		AddChild(_magnitudeBox);
		AddChild(new Label { Text = "->" }); AddChild(_targetBox);
	}

	private OptionButton CreateEnumDropdown<T>(T selected, Action<T> onChanged)
	{
		var box = new OptionButton();
		foreach (var val in Enum.GetNames(typeof(T)))
			box.AddItem(val.ToString());

		box.Selected = Convert.ToInt32(selected);
		box.ItemSelected += id => {
			var value = (T)Enum.GetValues(typeof(T)).GetValue(id);
			onChanged(value);
			Changed?.Invoke();
		};
		return box;
	}
}
