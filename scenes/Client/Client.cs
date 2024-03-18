using Godot;
using System;

public partial class Client : Node
{
    [Export]
    public long PeerID { get; set; } = -1;
    [Export]
    public bool IsLoggedIn { get; set; } = false;

    // Called when the node enters the scene tree for the first time.
    async public override void _Ready()
    {

        if (!Multiplayer.IsServer())
        {
            await ToSignal(GetTree(), "process_frame");
            // Some entities take a bit to get added to the tree, do not update them until then.
            if (!IsInsideTree())
            {
                await ToSignal(this, "tree_entered");
            }

            RpcId(1, "LoginToServer", "testuser", "testpassword");
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void LoginToServer(string username, string password)
    {
        GD.Print("Login request from: " + Multiplayer.GetRemoteSenderId() + " username: " + username);
        var id = Multiplayer.GetRemoteSenderId();

        IsLoggedIn = true;

        RpcId(id, "LoginResponse", true);
    }


    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void LoginResponse(bool response)
    {
        GD.Print("Login response: " + response);
    }
}
