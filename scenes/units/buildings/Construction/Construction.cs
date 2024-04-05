using Godot;
using System;

public partial class Construction : Unit
{
    [Export]
    public string BuildingName
    {
        get
        {
            return buildingName;
        }
        set
        {
            buildingName = value;
            Name = value + "-Construction";
            SetInfo();
        }
    }
    public override bool IsRepairable { get; set; } = true;
    private string buildingName = "";
    private Panel panel = null;
    private CollisionShape2D collisionShape = null;
    private float constructionTime = 0.0f;
    private float constructionProgress = 0.0f;

    public Construction()
    {
        AddReplicationProperty(".:BuildingName", true, SceneReplicationConfig.ReplicationMode.Never);
    }

    public override void _Ready()
    {
        base._Ready();
        panel = GetNode<Panel>("Panel");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");

        SetInfo();
    }

    private void SetInfo()
    {
        float radius;

        if (label == null || panel == null || collisionShape == null)
        {
            return;
        }

        label.Text = BuildingName + "-Construction: " + PlayerName;

        switch (BuildingName)
        {
            case "Townhall":
                radius = Townhall.Radius;
                constructionTime = Townhall.ConstructionTime;
                break;
            default:
                radius = Unit.Radius;
                constructionTime = Unit.ConstructionTime;
                break;
        }

        panel.Size = new Vector2(radius * 2, radius * 2);
        panel.Position = new Vector2(-radius, -radius);
        collisionShape.Shape = new CircleShape2D { Radius = radius };

    }

    public override bool GetRepaired(float amount)
    {
        constructionProgress += amount;

        if (constructionProgress >= constructionTime)
        {
            Player player = Map.GetPlayer(PlayerName);
            if (player != null)
            {
                Map.ServerCreateEntity(player, BuildingName, Position);
            }

            QueueFree();
        }
        return true;
    }
}
