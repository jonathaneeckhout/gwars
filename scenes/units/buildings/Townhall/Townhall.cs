using Godot;
using Godot.Collections;
using System;

public partial class Townhall : Unit
{
    public new const float Radius = 32.0f;
    public new const float ConstructionTime = 3.0f;
    public new const uint DefaultMaxHealth = 1000;
    public override bool IsRepairable { get; set; } = true;
    public override bool IsStorage { get; set; } = true;
    public override bool IsTrainingCenter { get; set; } = true;
    public new static readonly Dictionary<string, Dictionary> Units = new Dictionary<string, Dictionary>
    {
        { "Worker", new Dictionary { { "Gold", 0 }, {"Food", 100}, {"Time", 10.0}, { "Scene", GD.Load<PackedScene>("res://scenes/units/minions/Worker/Worker.tscn") } } }
    };
    [Export]
    public Array<string> TrainingQueue = new Array<string>();
    [Export]
    public float TrainingProgress = 0.0f;

    private Timer progressTimer = null;

    private ProgressBar progressBar = null;

    public Townhall()
    {
        MaxHealth = DefaultMaxHealth;
        Health = MaxHealth;

        AddReplicationProperty(".:TrainingQueue", true, SceneReplicationConfig.ReplicationMode.OnChange);
        AddReplicationProperty(".:TrainingProgress", true, SceneReplicationConfig.ReplicationMode.OnChange);
    }

    public override void _Ready()
    {
        base._Ready();

        progressTimer = new Timer();
        progressTimer.Name = "ProgressTimer";
        progressTimer.OneShot = false;
        progressTimer.WaitTime = 1.0f;
        progressTimer.Autostart = false;
        progressTimer.Timeout += OnProgressTimerTimeout;
        AddChild(progressTimer);

        progressBar = GetNode<ProgressBar>("ProgressBar");

        if (Multiplayer.IsServer())
        {
            SetProcess(false);
        }
    }

    public override void _Process(double delta)
    {
        if (TrainingQueue.Count > 0)
        {
            progressBar.Value = TrainingProgress / (float)Units[(string)TrainingQueue[0]]["Time"] * 100.0f;
        }
        else
        {
            progressBar.Value = 0.0f;
        }

    }

    public override bool StoreMaterial(string materialType, uint amount)
    {
        Player player = Map.GetPlayer(PlayerName);
        return player.AddMaterial(materialType, amount);
    }


    public override bool TrainUnit(string unitType)
    {
        if (Units.ContainsKey(unitType))
        {
            Player player = Map.GetPlayer(PlayerName);
            if (player != null)
            {
                if (player.RemoveMaterials((uint)Units[unitType]["Gold"], (uint)Units[unitType]["Food"]))
                {
                    TrainingQueue.Add(unitType);

                    if (progressTimer.IsStopped())
                    {
                        progressTimer.Start();
                    }

                    return true;
                }
            }
        }

        return false;
    }

    private void OnProgressTimerTimeout()
    {
        if (TrainingQueue.Count > 0)
        {
            TrainingProgress += (float)progressTimer.WaitTime;
            if (TrainingProgress >= (float)Units[(string)TrainingQueue[0]]["Time"])
            {
                Player player = Map.GetPlayer(PlayerName);
                if (player != null)
                {
                    Map.ServerCreateEntity(player, (string)TrainingQueue[0], Position + new Vector2(0, 64));
                }
                TrainingQueue.RemoveAt(0);
                TrainingProgress = 0.0f;
            }
        }

        if (TrainingQueue.Count == 0)
        {
            progressTimer.Stop();
        }
    }
}
