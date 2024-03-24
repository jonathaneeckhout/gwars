using Godot;
using System;

public partial class InputComponent : Node2D
{
    public UnitSelectionComponent UnitSelectionComponent { get; set; } = null;
    public GroupComponent GroupComponent { get; set; } = null;


    private bool numSelected = false;

    // Called when the node enters the scene tree for the first time.

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("num1"))
        {
            numSelected = !numSelected;
            UnitSelectionComponent.Enabled = !numSelected;

            GD.Print("Num1 pressed");
        }
        else if (Input.IsActionJustReleased("right_click"))
        {
            GroupComponent.RpcId(1, GroupComponent.MethodName.MoveGroupRPC, GetGlobalMousePosition());
        }
    }
}
