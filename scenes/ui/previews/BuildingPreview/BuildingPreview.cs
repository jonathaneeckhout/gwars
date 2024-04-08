using Godot;
using System;

public partial class BuildingPreview : Area2D
{
    public string BuildingName
    {
        get
        {
            return buildingName;
        }
        set
        {
            buildingName = value;
            Name = buildingName;
            label.Text = buildingName;
            SetPanelSize();
        }

    }
    private string buildingName = "";
    private Label label = null;
    private Panel panel = null;
    private CollisionShape2D collisionShape = null;
    private uint bodyCollisionCount = 0;

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        panel = GetNode<Panel>("Panel");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        BodyEntered += OnBodyEntered;
        BodyExited += OnBodyExited;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        {
            Position = Map.SnapToGrid(GetGlobalMousePosition());
        }
    }

    private void SetPanelSize()
    {
        float radius = buildingName switch
        {
            "Townhall" => Townhall.Radius,
            _ => Unit.Radius,
        };
        panel.Size = new Vector2(radius * 2, radius * 2);
        panel.Position = new Vector2(-radius, -radius);
        collisionShape.Shape = new CircleShape2D { Radius = radius };
    }

    private void OnBodyEntered(Node body)
    {
        bodyCollisionCount++;

        panel.Modulate = new Color(1.0f, 0.0f, 0.0f, 1.0f);
    }

    private void OnBodyExited(Node body)
    {
        if (bodyCollisionCount > 0)
        {
            bodyCollisionCount--;
        }

        if (bodyCollisionCount == 0)
        {
            panel.Modulate = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        }
    }
}
