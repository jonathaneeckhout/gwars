using Godot;
using System;

public partial class CameraMovementComponent : Node
{
    public const float Speed = 50.0f;
    public const float MinZoom = 0.5f;
    public const float MaxZoom = 2.0f;
    public const float ZoomFactor = 0.1f;
    public const float ZoomDuration = 0.2f;
    private Camera2D camera = null;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        camera = GetParent<Camera2D>();
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 inputDirection = Input.GetVector("move_left", "move_right", "move_up", "move_down");
        camera.Position += inputDirection * Speed;
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("zoom_in"))
        {
            SetZoomLevel(camera.Zoom.X + ZoomFactor);
        }
        else if (Input.IsActionJustPressed("zoom_out"))
        {
            SetZoomLevel(camera.Zoom.X - ZoomFactor);
        }
    }

    void SetZoomLevel(float value)
    {
        float zoomLevel = Mathf.Clamp(value, MinZoom, MaxZoom);
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(camera, "zoom", new Vector2(zoomLevel, zoomLevel), ZoomDuration);
    }
}
