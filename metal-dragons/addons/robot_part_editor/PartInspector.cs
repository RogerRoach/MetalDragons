using Godot;
using System;
using System.Linq;

public partial class PartInspector : VBoxContainer
{
	private RobotPart _part;
	private LineEdit _nameEdit;
	private SpinBox _tierEdit, _levelEdit, _atkEdit, _hpEdit;
	private HBoxContainer _familyEdits;
	private OptionButton _familyEdit;
	private OptionButton _family2Edit;
	private OptionButton _family3Edit;
	private VBoxContainer _effectsContainer;
	private OptionButton _effectTypeSelector;
	private Label _previewLabel;

	private ConfirmationDialog _deleteEffectDialog;
	private GameEffect _effectToDelete;
	
	public event Action Changed;

	public PartInspector()
	{
		Hide();
	}

	private void OnChanged()
	{
		try
		{
			Changed?.Invoke();
		}
		catch (Exception ex)
		{
			GD.PrintErr("Changed event failed: ", ex.Message);
		}
	}

	public override void _Ready()
	{
		_nameEdit = AddField("Name", new LineEdit { CustomMinimumSize = new Vector2(180, 24), SizeFlagsHorizontal = Control.SizeFlags.ShrinkBegin });
		_tierEdit = AddField("Tier", new SpinBox { MinValue = 1, MaxValue = 10 });
		_levelEdit = AddField("Level", new SpinBox { MinValue = 1, MaxValue = 99 });
		_atkEdit = AddField("Attack", new SpinBox());
		_hpEdit = AddField("HP", new SpinBox());

		_familyEdits = new HBoxContainer();
		AddChild(_familyEdits);

		_familyEdit = AddField("Family", new OptionButton(), _familyEdits);
		foreach (var val in Enum.GetNames(typeof(PartFamily)))
			_familyEdit.AddItem(val.ToString());
			
		_family2Edit = AddField("Family2", new OptionButton(), _familyEdits);
		foreach (var val in Enum.GetNames(typeof(PartFamily)))
			_family2Edit.AddItem(val.ToString());
			
		_family3Edit = AddField("Family3", new OptionButton(), _familyEdits);
		foreach (var val in Enum.GetNames(typeof(PartFamily)))
			_family3Edit.AddItem(val.ToString());

		AddChild(new Label { Text = "Effects", ThemeTypeVariation = "Header" });
		_effectsContainer = new VBoxContainer();
		AddChild(_effectsContainer);

		_previewLabel = new Label { AutowrapMode = TextServer.AutowrapMode.WordSmart };
		AddChild(_previewLabel);
		
		_nameEdit.TextChanged += OnNameChanged;
		_tierEdit.ValueChanged += v => { if (_part != null && _part.Tier != (int)v) _part.Tier = (int)v; OnChanged(); };
		_levelEdit.ValueChanged += v => { if (_part != null && _part.Level != (int)v) _part.Level = (int)v; OnChanged(); };
		_atkEdit.ValueChanged += v => { if (_part != null && _part.EquippedAttack != (int)v) _part.EquippedAttack = (int)v; OnChanged(); };
		_hpEdit.ValueChanged += v => { if (_part != null && _part.EquippedHP != (int)v) _part.EquippedHP = (int)v; OnChanged(); };
		_familyEdit.ItemSelected += i => { if (_part != null && _part.Family != (PartFamily)i) _part.Family = (PartFamily)i; OnChanged(); };
		
		_deleteEffectDialog = new ConfirmationDialog
		{
			DialogText = "Are you sure you want to delete this effect?",
			Visible = false
		};
		_deleteEffectDialog.Confirmed += OnDeleteConfirmed;
		
		AddChild(_deleteEffectDialog);
	}

	private void OnNameChanged(string newName)
	{
		if (_part == null)
		{
			Hide();
			return;
		}
		
		Show();
		
		if (_part.Name == newName) return;
		_part.Name = newName;
		OnChanged();
	}

	private T AddField<T>(string name, T control) where T : Control
	{
		return AddField(name, control, this);
	}

	private T AddField<T>(string name, T control, Container parent)  where T : Control
	{
		var hbox = new HBoxContainer();
		hbox.AddChild(new Label { Text = name, CustomMinimumSize = new Vector2(100, 0) });
		hbox.AddChild(control);
		parent.AddChild(hbox);
		return control;
	}

	public void SetPart(RobotPart part)
	{
		if (part == null)
		{
			Hide();
			return;
		}
		
		Show();
		
		_part = part;
		_nameEdit.Text = part.Name;
		_tierEdit.Value = part.Tier;
		_levelEdit.Value = part.Level;
		_atkEdit.Value = part.EquippedAttack;
		_hpEdit.Value = part.EquippedHP;
		_familyEdit.Selected = (int)part.Family;

		RefreshEffects();
		UpdatePreview();
	}

	private void ClearChildren(Container container)
	{
		foreach (var child in container.GetChildren())
			((Node)child).QueueFree();
	}

	private void RefreshEffects()
	{
		ClearChildren(_effectsContainer);

		foreach (var eff in _part.Effects)
		{
			var editor = new EffectEditor(eff);
			editor.Changed += UpdatePreview;
			var deleteButton = new Button { Text = "Delete" };
			deleteButton.Pressed += () =>
			{
				_effectToDelete = eff;
				_deleteEffectDialog.PopupCentered();
			};
			editor.AddChild(deleteButton);
			_effectsContainer.AddChild(editor);
		}

		var hbox = new HBoxContainer();

		var addBtn = new Button { Text = "Add Effect" };
		addBtn.Pressed += OnAddEffectPressed;
		hbox.AddChild(addBtn);
		
		_effectTypeSelector = new OptionButton();
		_effectTypeSelector.AddItem("Single Target", 0);
		_effectTypeSelector.AddItem("Summon", 1);
		hbox.AddChild(_effectTypeSelector);
		
		_effectsContainer.AddChild(hbox);
	}

	private void OnDeleteConfirmed()
	{
		if (_part == null || _effectToDelete == null)
		{
			return;
		}
		
		_part.Effects.Remove(_effectToDelete);
		_effectToDelete = null;
		RefreshEffects();
	}

	private void OnAddEffectPressed()
	{
		if (_part == null) return;

		GameEffect newEffect = _effectTypeSelector.GetSelectedId() switch
		{
			0 => new SingleTargetGameEffect
			{
				Trigger = (Trigger)0,
				Target = (Target)0,
				Effect = new Effect((EffectType)0, 10)
			},
			1 => new SummonGameEffect
			{
				Trigger = (Trigger)0,
				UnitToSummon = "Drone",
				SummonCount = 1
			},
			_ => throw new InvalidOperationException("Unknown effect type")
		};
	
		_part.Effects.Add(newEffect);		
		
		RefreshEffects();
	}

	private void UpdatePreview()
	{
		if (_part == null) return;
		var text = string.Join("\n", _part.Effects.Select(e =>
		{
			if (e is SingleTargetGameEffect s)
				return $"On {s.Trigger}: {s.Effect.Type}({s.Effect.Magnitude}) → {s.Target}";
			else if (e is SummonGameEffect sum)
				return $"On {sum.Trigger} summon {sum.SummonCount}x {sum.UnitToSummon} to {sum.Target}";
			return e.Trigger.ToString();
		}));
		_previewLabel.Text = text;
	}
}
