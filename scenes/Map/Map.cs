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

        players.ChildEnteredTree += OnPlayersChildEnteredTree;
    }

    public void ServerCreateWorker(Vector2 position)
    {
        Worker worker = (Worker)workerScene.Instantiate();
        worker.Position = position;
        units.AddChild(worker, true);
    }
    private void OnServerClientLoggedIn(Client client)
    {
        Player player = (Player)playerScene.Instantiate();
        player.Map = this;
        player.Username = client.Username;
        player.PeerID = client.PeerID;
        players.AddChild(player, true);
    }

    private void OnPlayersChildEnteredTree(Node child)
    {
        GD.Print("Child entered tree");
        if (child is Player player)
        {
            GD.Print("Setting player map");
            player.Map = this;
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void CreateWorkerRPC(Vector2 position)
    {
        if (!Multiplayer.IsServer())
        {
            GD.Print("CreateWorker called on non-server node. Ignoring.");
            return;
        }

        ServerCreateWorker(position);
    }
}
