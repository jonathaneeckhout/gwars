using Godot;
using System;

public partial class TrainUnitButton : Button
{
    public GroupComponent GroupComponent { get; set; } = null;
    public Unit TrainingCenter { get; set; } = null;
    public string UnitType { get; set; } = "";

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Pressed += OnPressed;
    }
    private void OnPressed()
    {
        GD.Print("Train unit: " + UnitType);
        GroupComponent.RpcId(1, GroupComponent.MethodName.TrainUnitGroupRPC, TrainingCenter.Name, UnitType);
    }
}
