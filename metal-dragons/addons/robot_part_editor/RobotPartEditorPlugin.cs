using Godot;

[Tool]
public partial class RobotPartEditorPlugin : EditorPlugin
{
	private Control _dock;

	public override void _EnterTree()
	{
		_dock = GD.Load<PackedScene>("res://addons/robot_part_editor/RobotPartEditor.tscn").Instantiate<Control>();
		AddControlToDock(DockSlot.RightUl, _dock);
	}

	public override void _ExitTree()
	{
		RemoveControlFromDocks(_dock);
		_dock.QueueFree();
	}
}
