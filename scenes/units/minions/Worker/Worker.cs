using Godot;
using System;

public partial class Worker : Unit
{
    public const float Speed = 300.0f;

    public override void _Ready()
    {
        base._Ready();

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

    public override void MoveTo(Vector2 position)
    {
        targetPosition = position;
    }
}
