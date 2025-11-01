using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class RobotPartEditor : HBoxContainer
{
	private VBoxContainer _partListContainer;
	private VBoxContainer _inspectorContainer;
	private VBoxContainer _buttonContainer;
	private PartInspector _inspector;
	private List<RobotPart> _parts = new();
	private RobotPart _selectedPart;
	private Button _saveButton;
	private Button _deleteButton;
	private ConfirmationDialog _deleteDialog;

	public override void _Ready()
	{
		_parts = RobotPartCache.Load();
		BuildUI();
		RefreshPartList();
	}

	private void BuildUI()
	{
		_buttonContainer = new VBoxContainer();
		
		var addButton = new Button { Text = "Add Part" };
		addButton.Pressed += OnAddPartPressed;
		_buttonContainer.AddChild(addButton);

		_saveButton = new Button { Text = "Save" };
		_saveButton.Pressed += OnSavePressed;
		_buttonContainer.AddChild(_saveButton);
		
		_deleteButton = new Button { Text = "Delete" };
		_deleteButton.Disabled = true; // disabled until a part is selected
		_deleteButton.Pressed += OnDeletePressed;
		_buttonContainer.AddChild(_deleteButton);
		
		AddChild(_buttonContainer);
		
		_partListContainer = new VBoxContainer();
		
		var scrollContainer = new ScrollContainer
		{
			CustomMinimumSize = new Vector2(250, 400), // Adjust size as needed
			SizeFlagsVertical = Control.SizeFlags.ExpandFill
		};
		
		scrollContainer.AddThemeConstantOverride("scroll_bar_width", 8);
		scrollContainer.ScrollVertical = 0; // reset scroll to top when refreshing
		
		scrollContainer.AddChild(_partListContainer);
		AddChild(scrollContainer);
		
		_inspectorContainer = new VBoxContainer { SizeFlagsHorizontal = SizeFlags.ExpandFill };
		AddChild(_inspectorContainer);

		_inspector = new PartInspector();
		_inspectorContainer.AddChild(_inspector);
		
		_deleteDialog = new ConfirmationDialog
		{
			DialogText = "Are you sure you want to delete this part?",
			Visible = false
		};
		AddChild(_deleteDialog);
		_deleteDialog.Confirmed += OnDeleteConfirmed;
	}

	private void RefreshPartList()
	{
		for (int i = _partListContainer.GetChildCount() - 1; i >= 0; i--)
		{
			var child = _partListContainer.GetChild(i);
			_partListContainer.RemoveChild(child);
			
			child.QueueFree();
		}
	
		foreach (var part in _parts.OrderBy(p => p.Name))
		{
			var button = new Button { Text = $"{part.Name} (T{part.Tier})" };
			if (part == _selectedPart)
				button.Modulate = new Color(0.8f, 1f, 0.8f); // highlight selected
			button.Pressed += () =>
			{
				UpdateSelectedPart(part);
				RefreshPartList();
			};
			_partListContainer.AddChild(button);
		}
	}

	public void UpdateSelectedPart(RobotPart part)
	{
		_selectedPart = part;
		_deleteButton.Disabled = _selectedPart == null;
		_inspector.SetPart(part);
	}

	private void OnDeletePressed()
	{
		if (_selectedPart == null)
			return;

		_deleteDialog.DialogText = $"Delete '{_selectedPart.Name}'?";
		_deleteDialog.PopupCentered();
	}

	private void OnDeleteConfirmed()
	{
		if (_selectedPart == null)
			return;

		_parts.Remove(_selectedPart);
		_selectedPart = null;
		RefreshPartList();

		UpdateSelectedPart(null);
		GD.Print("Part deleted");
	}

	private void OnAddPartPressed()
	{
		try
		{
			var newPart = new RobotPart
			{
				Name = "New Part",
				Tier = 1,
				Level = 1,
				Family = (PartFamily)0,
				EquippedAttack = 1,
				EquippedHP = 1,
				Effects = new List<GameEffect>()
			};

			_parts.Add(newPart);
			_selectedPart = newPart;
			RefreshPartList();
		}
		catch (Exception ex)
		{
			GD.PrintRich("Error adding part: ", ex.Message, "\n", ex.StackTrace);
		}
	}
	
	private void OnSavePressed()
	{
		RobotPartCache.Save(_parts);
	}
}
