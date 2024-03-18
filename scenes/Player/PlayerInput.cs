using Godot;
using System;

public partial class PlayerInput : MultiplayerSynchronizer
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Only process for the local player.
        SetProcessInput(GetMultiplayerAuthority() == Multiplayer.GetUniqueId());
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
