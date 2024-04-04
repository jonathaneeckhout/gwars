using Godot;
using Godot.Collections;
using System;

public partial class Townhall : Unit
{
    public new const float Radius = 32.0f;
    public new const float ConstructionTime = 3.0f;
    public override bool IsRepairable { get; set; } = true;
    public override bool IsStorage { get; set; } = true;
    public override bool IsTrainingCenter { get; set; } = true;

    public new static readonly Dictionary<string, Dictionary> Units = new Dictionary<string, Dictionary>
    {
        { "Worker", new Dictionary { { "Gold", 0 }, {"Food", 100}, {"Time", 3.0}, { "Scene", GD.Load<PackedScene>("res://scenes/units/minions/Worker/Worker.tscn") } } }
    };
    private Array<Dictionary> trainingQueue = new Array<Dictionary>();

    private Timer progressTimer = null;

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
                    Vector2 SpawnPosition = Position + new Vector2(0, 64);
                    // TODO: add unit to training queue

                    trainingQueue.Add(new Dictionary { { "UnitType", unitType }, { "Progress", 0.0 } });

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
        if (trainingQueue.Count > 0)
        {
            trainingQueue[0]["Progress"] = (float)trainingQueue[0]["Progress"] + progressTimer.WaitTime;
            if ((float)trainingQueue[0]["Progress"] >= (float)Units[(string)trainingQueue[0]["UnitType"]]["Time"])
            {
                Player player = Map.GetPlayer(PlayerName);
                if (player != null)
                {
                    Map.ServerCreateEntity(player, (string)trainingQueue[0]["UnitType"], Position + new Vector2(0, 64));
                }
                trainingQueue.RemoveAt(0);
            }
        }

        if (trainingQueue.Count == 0)
        {
            progressTimer.Stop();
        }
    }
}
