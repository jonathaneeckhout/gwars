using Godot;
using System;

public partial class Client : Node
{
    [Export]
    public long PeerID { get; set; } = -1;
    [Export]
    public string Username { get; set; } = "";
    [Export]
    public bool IsLoggedIn { get; set; } = false;

    // Called when the node enters the scene tree for the first time.
    async public override void _Ready()
    {

        // if (!Multiplayer.IsServer())
        // {
        //     await ToSignal(GetTree(), "process_frame");
        //     // Some entities take a bit to get added to the tree, do not update them until then.
        //     if (!IsInsideTree())
        //     {
        //         await ToSignal(this, "tree_entered");
        //     }

        //     RpcId(1, "LoginToServer", "testuser", "testpassword");
        // }
    }
}
