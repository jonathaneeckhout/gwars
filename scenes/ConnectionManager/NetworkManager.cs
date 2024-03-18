using Godot;
using System;

public partial class NetworkManager : Node
{
    [Signal] public delegate void ClientConnectedEventHandler(bool result);

    [Signal] public delegate void ClientLoggedInEventHandler(bool result);
    private ENetMultiplayerPeer server = null;
    private ENetMultiplayerPeer client = null;

    private Node clients = null;

    private PackedScene clientScene = GD.Load<PackedScene>("res://scenes/Client/Client.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        clients = GetNode<Node>("%Clients");
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

    private void ServerAddClient(long id)
    {
        var client = clientScene.Instantiate() as Client;
        client.PeerID = id;
        client.Name = $"Client{id}";
        clients.AddChild(client);
    }

    private void ServerRemoveClient(long id)
    {
        if (clients.HasNode($"Client{id}"))
        {
            var client = clients.GetNode<Client>($"Client{id}");
            client.QueueFree();
        }
    }

    private Client ServerGetClient(long id)
    {
        if (clients.HasNode($"Client{id}"))
        {
            return clients.GetNode<Client>($"Client{id}");
        }

        return null;
    }

    public bool StartClient(string address, int port)
    {
        if (client != null)
        {
            GD.Print("Client already started");
            return true;
        }

        GD.Print("Starting as client, connecting to server at " + address + ":" + port);

        client = new ENetMultiplayerPeer();
        var error = client.CreateClient(address, port);
        if (error != Error.Ok)
        {
            GD.Print($"Error: {error}");
            return false;
        }

        Multiplayer.ConnectedToServer += OnConnectedToServer;
        Multiplayer.ConnectionFailed += OnConnectionFailed;
        Multiplayer.ServerDisconnected += OnServerDisconnected;

        Multiplayer.MultiplayerPeer = client;

        return true;
    }

    public void CloseClient()
    {
        if (client != null)
        {
            Multiplayer.ConnectedToServer -= OnConnectedToServer;
            Multiplayer.ConnectionFailed -= OnConnectionFailed;
            Multiplayer.ServerDisconnected -= OnServerDisconnected;

            client.Close();

            Multiplayer.MultiplayerPeer = null;
            client = null;
        }
    }

    private void OnPeerConnected(long id)
    {
        GD.Print($"Peer connected: {id}");

        ServerAddClient(id);
    }

    private void OnPeerDisconnected(long id)
    {
        GD.Print($"Peer disconnected: {id}");

        ServerRemoveClient(id);
    }

    private void OnConnectedToServer()
    {
        GD.Print("Connected to server");

        EmitSignal(SignalName.ClientConnected, true);
    }

    private void OnConnectionFailed()
    {
        GD.Print("Connection failed");

        CloseClient();

        EmitSignal(SignalName.ClientConnected, false);
    }

    private void OnServerDisconnected()
    {
        GD.Print("Server disconnected");

        CloseClient();

        EmitSignal(SignalName.ClientConnected, false);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void LoginToServer(string username, string password)
    {
        GD.Print("Login request from: " + Multiplayer.GetRemoteSenderId() + " username: " + username);
        var id = Multiplayer.GetRemoteSenderId();

        var client = ServerGetClient(id);
        if (client == null)
        {
            GD.Print("Client not found");
            return;
        }

        // TODO: Check username and password
        client.Username = username;
        client.IsLoggedIn = true;

        RpcId(id, "LoginResponse", client.IsLoggedIn);
    }


    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void LoginResponse(bool response)
    {
        GD.Print("Login response: " + response);
        EmitSignal(SignalName.ClientLoggedIn, response);
    }
}
