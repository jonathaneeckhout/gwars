using Godot;
using System;

public partial class NetworkManager : Node
{
    private ENetMultiplayerPeer server = null;
    private ENetMultiplayerPeer client = null;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void StartServer(int port)
    {
        GD.Print("Starting as server");
        server = new ENetMultiplayerPeer();

        var error = server.CreateServer(port);
        if (error != Error.Ok)
        {
            GD.Print($"Error: {error}");
            return;
        }

        Multiplayer.PeerConnected += OnPeerConnected;

        Multiplayer.PeerDisconnected += OnPeerDisconnected;

        Multiplayer.MultiplayerPeer = server;

        GD.Print("Server started");

    }



    private void OnPeerConnected(long id)
    {
        GD.Print($"Peer connected: {id}");
    }

    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Peer disconnected: {id}");
    }

    public void StartClient(string address, int port)
    {
        GD.Print("Starting as client");

        client = new ENetMultiplayerPeer();
        var error = client.CreateClient(address, port);
        if (error != Error.Ok)
        {
            GD.Print($"Error: {error}");
            return;
        }

        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ServerDisconnected += OnServerDisconnected;
        Multiplayer.MultiplayerPeer = client;
    }

    private void OnConnectedToServer()
    {
        GD.Print("Connected to server");
    }

    private void OnServerDisconnected()
    {
        GD.Print("Server disconnected");
    }
}
