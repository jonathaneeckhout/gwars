using Godot;
using Godot.Collections;
using System;

public partial class Unit : CharacterBody2D
{
    public const float Radius = 16.0f;
    public const float ConstructionTime = 0.0f;

    [Export]
    public string PlayerName { get; set; } = "";

    public Map Map { get; set; } = null;
    public virtual bool IsRepairable { get; set; } = false;
    public virtual bool IsStorage { get; set; } = false;
    public virtual bool IsTrainingCenter { get; set; } = false;
    public static readonly Dictionary<string, Dictionary> Units = new Dictionary<string, Dictionary>();
    protected MultiplayerSynchronizer synchronizer = null;
    protected SceneReplicationConfig sceneReplicationConfig = null;
    protected Vector2 targetPosition = Vector2.Zero;

    protected Panel selectionPanel = null;

    public Unit()
    {
        sceneReplicationConfig = new SceneReplicationConfig();
        AddReplicationProperty(".:position", true, SceneReplicationConfig.ReplicationMode.OnChange);
        AddReplicationProperty(".:PlayerName", true, SceneReplicationConfig.ReplicationMode.OnChange);

        synchronizer = new MultiplayerSynchronizer();
        synchronizer.Name = "Synchronizer";
        synchronizer.ReplicationInterval = 0.1f;
        synchronizer.DeltaInterval = 0.1f;
        synchronizer.ReplicationConfig = sceneReplicationConfig;
        AddChild(synchronizer);
    }

    public override void _Ready()
    {
        targetPosition = Position;
        selectionPanel = GetNodeOrNull<Panel>("%SelectedPanel");

        if (!Multiplayer.IsServer())
        {
            SetPhysicsProcess(false);
        }
    }

    public virtual void SetSelected(bool value)
    {
        if (selectionPanel != null)
        {
            selectionPanel.Visible = value;
        }
    }

    public virtual void MoveTo(Vector2 position) { }

    public virtual void GatherMaterial(Material material, Unit Storage) { }

    public virtual bool StoreMaterial(string materialType, uint amount)
    {
        return false;
    }

    public virtual void RepairTarget(Unit target) { }

    public virtual bool GetRepaired(float amount)
    {
        return false;
    }

    public virtual bool TrainUnit(string unitType)
    {
        return false;
    }

    public void AddReplicationProperty(string property, bool spawn = true, SceneReplicationConfig.ReplicationMode mode = SceneReplicationConfig.ReplicationMode.OnChange)
    {
        sceneReplicationConfig.AddProperty(property);
        sceneReplicationConfig.PropertySetSpawn(property, spawn);
        sceneReplicationConfig.PropertySetReplicationMode(property, mode);
    }
}
