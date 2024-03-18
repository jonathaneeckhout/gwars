using Godot;
using System;

public partial class Map : Node2D
{
    private PackedScene playerScene = GD.Load<PackedScene>("res://scenes/Player/Player.tscn");
    private PackedScene workerScene = GD.Load<PackedScene>("res://scenes/units/Worker/Worker.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void CreateWorker(Vector2 position)
    {
        var worker = workerScene.Instantiate() as Worker;
        worker.Position = position;
        AddChild(worker);
    }
}
