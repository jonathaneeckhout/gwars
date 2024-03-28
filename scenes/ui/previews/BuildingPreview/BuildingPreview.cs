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

    public override void _Ready()
    {
        label = GetNode<Label>("Label");
        panel = GetNode<Panel>("Panel");
    }

    public override void _Process(double delta)
    {
        if (Visible)
        {
            Position = GetGlobalMousePosition();
        }
    }

    private void SetPanelSize()
    {
        float radius = 0.0f;

        switch (buildingName)
        {
            case "Townhall":
                radius = Townhall.Radius;
                break;
            default:
                radius = Unit.Radius;
                break;
        }
        panel.Size = new Vector2(radius * 2, radius * 2);
    }
}
