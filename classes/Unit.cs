using Godot;
using Godot.Collections;
using System;

public partial class Unit : CharacterBody2D
{
    public const float Radius = 16.0f;
    public const float ConstructionTime = 0.0f;

    public const uint DefaultMaxHealth = 100;

    [Export]
    public string PlayerName { get; set; } = "";
    [Export]
    public uint MaxHealth { get; set; } = 100;
    [Export]
    public uint Health
    {
        get
        {
            return health;
        }
        set
        {
            health = value;
            UpdateHealtBar();
        }
    }
    public uint AttackPower { get; set; } = 10;
    public float AttackSpeed { get; set; } = 1.0f;
    public uint DefensePower { get; set; } = 0;
    public Map Map { get; set; } = null;
    public virtual bool IsRepairable { get; set; } = false;
    public virtual bool IsStorage { get; set; } = false;
    public virtual bool IsTrainingCenter { get; set; } = false;
    public static readonly Dictionary<string, Dictionary> Units = new Dictionary<string, Dictionary>();
    protected MultiplayerSynchronizer synchronizer = null;
    protected SceneReplicationConfig sceneReplicationConfig = null;
    protected Vector2 targetPosition = Vector2.Zero;

    protected Label label = null;
    protected Panel selectionPanel = null;
    protected ProgressBar healthBar = null;

    private uint health = 100;
    public Unit()
    {
        sceneReplicationConfig = new SceneReplicationConfig();
        AddReplicationProperty(".:position", true, SceneReplicationConfig.ReplicationMode.OnChange);
        AddReplicationProperty(".:PlayerName", true, SceneReplicationConfig.ReplicationMode.OnChange);
        AddReplicationProperty(".:MaxHealth", true, SceneReplicationConfig.ReplicationMode.OnChange);
        AddReplicationProperty(".:Health", true, SceneReplicationConfig.ReplicationMode.OnChange);

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
        label = GetNodeOrNull<Label>("%Label");
        selectionPanel = GetNodeOrNull<Panel>("%SelectedPanel");
        healthBar = GetNodeOrNull<ProgressBar>("%HealthBar");

        if (!Multiplayer.IsServer())
        {
            SetPhysicsProcess(false);
        }

        UpdateHealtBar();

        if (label != null)
        {
            label.Text = label.Text + ": " + PlayerName;
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

    public virtual void Hurt(uint damage)
    {
        Health -= Math.Max(0, damage - DefensePower);

        Health = Math.Max(0, Math.Min(Health, MaxHealth));

        UpdateHealtBar();

        if (Health <= 0)
        {
            QueueFree();
        }
    }

    public virtual void Heal(uint amount)
    {
        Health += amount;

        Health = Math.Max(0, Math.Min(Health, MaxHealth));

        UpdateHealtBar();
    }

    public virtual void AttackUnit(Unit target) { }
    public virtual void GatherMaterial(Material material, Unit Storage) { }

    public virtual bool StoreMaterial(string materialType, uint amount)
    {
        return false;
    }

    public virtual void RepairTarget(Unit target) { }

    public virtual bool GetRepaired(uint amount)
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

    private void UpdateHealtBar()
    {
        if (healthBar != null)
        {
            healthBar.MaxValue = MaxHealth;
            healthBar.Value = Health;
        }
    }
}
