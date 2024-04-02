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
        { "Worker", new Dictionary { { "Gold", 0 }, {"food", 100}, { "Scene", GD.Load<PackedScene>("res://scenes/units/minions/Worker/Worker.tscn") } } }
    };

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
                    // TODO: add unit to training queue
                    Map.ServerCreateEntity(player, unitType, Position);
                    return true;
                }
            }
        }

        return false;
    }
}
