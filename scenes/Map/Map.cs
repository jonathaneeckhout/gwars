using Godot;
using System;

public partial class Map : Node2D
{
    private NetworkManager networkManager = null;
    public NetworkManager NetworkManager
    {
        set
        {
            networkManager = value;

            if (networkManager != null)
            {
                if (Multiplayer.IsServer())
                {
                    networkManager.ServerClientLoggedIn += OnServerClientLoggedIn;
                }
            }
        }
    }

    private Node2D units = null;
    private Node2D players = null;
    private PackedScene playerScene = GD.Load<PackedScene>("res://scenes/Player/Player.tscn");
    private PackedScene workerScene = GD.Load<PackedScene>("res://scenes/units/Worker/Worker.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        units = GetNode<Node2D>("%Units");
        players = GetNode<Node2D>("%Players");

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void CreateWorker(Vector2 position)
    {
        var worker = workerScene.Instantiate() as Worker;
        worker.Position = position;
        units.AddChild(worker);
    }
    private void OnServerClientLoggedIn(Client client)
    {
        var player = playerScene.Instantiate() as Player;
        player.Username = client.Username;
        player.PeerID = client.PeerID;
        players.AddChild(player);
    }
}
