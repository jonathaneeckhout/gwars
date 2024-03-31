using Godot;
using System;

public partial class Townhall : Unit
{
    public new const float Radius = 32.0f;
    public override bool IsRepairable { get; set; } = true;
    public override bool IsStorage { get; set; } = true;

    public override bool StoreMaterial(string materialType, uint amount)
    {
        Player player = Map.GetPlayer(PlayerName);
        return player.AddMaterial(materialType, amount);
    }
}
