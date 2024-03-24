using Godot;
using System;

public partial class Worker : Unit
{
    public const float Speed = 300.0f;

    private Panel selectionPanel = null;

    private Vector2 targetPosition = Vector2.Zero;

    public override void _Ready()
    {
        targetPosition = Position;
        selectionPanel = GetNode<Panel>("%SelectedPanel");

        if (!Multiplayer.IsServer())
        {
            SetPhysicsProcess(false);
        }
    }


    public override void _PhysicsProcess(double delta)
    {
        if (Position.DistanceTo(targetPosition) > 8.0f)
        {
            Velocity = Position.DirectionTo(targetPosition) * Speed;

        }
        else
        {
            Velocity = Vector2.Zero;
        }

        MoveAndSlide();
    }


    public override void SetSelected(bool value)
    {
        selectionPanel.Visible = value;
    }

    public override void MoveTo(Vector2 position)
    {
        GD.Print("Moving worker");
        targetPosition = position;
    }

}
