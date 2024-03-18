using Godot;
using System;

public partial class Main : Node2D
{
    private const string server_address = "127.0.0.1";
    private const int server_port = 9876;

    private CanvasLayer uiCanvasLayer;

    private NetworkManager networkManager;
    private Control networkControl = null;

    private ConnectScreen connectScreen = null;

    private LoginScreen loginScreen = null;

    private PackedScene mapScene = GD.Load<PackedScene>("res://scenes/Map/Map.tscn");

    private PackedScene connectScene = GD.Load<PackedScene>("res://scenes/ui/ConnectScreen/ConnectScreen.tscn");

    private PackedScene loginScreenScene = GD.Load<PackedScene>("res://scenes/ui/LoginScreen/LoginScreen.tscn");

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

        uiCanvasLayer = GetNode<CanvasLayer>("UICanvasLayer");

        // Get the network manager
        networkManager = GetNode<NetworkManager>("%NetworkManager");

        // Get the network control panel
        networkControl = GetNode<Control>("%NetworkControl");

        // Get the server button and add a listener
        Button serverButton = GetNode<Button>("%ServerButton");
        serverButton.Pressed += OnServerButtonPressed;


        // Get the network manager and start the server
        Button clientButton = GetNode<Button>("%ClientButton");
        clientButton.Pressed += OnClientButtonPressed;

    }

    // Called when the server button is pressed
    private void OnServerButtonPressed()
    {
        GD.Print("Server button pressed");
        StartServer();

    }

    // Called when the client button is pressed
    private void OnClientButtonPressed()
    {
        GD.Print("Client button pressed");
        StartClient();
    }

    private void StartServer()
    {
        GetWindow().Title = "GWars Server";

        GetTree().Root.Mode = Window.ModeEnum.Minimized;

        networkControl.Hide();
        networkControl.QueueFree();

        networkManager.StartServer(server_port);

        var map = mapScene.Instantiate() as Map;
        AddChild(map);
    }

    private void StartClient()
    {
        GetWindow().Title = "GWars Client";


        if (networkControl != null)
        {
            networkControl.Hide();
            networkControl.QueueFree();
            networkControl = null;
        }

        if (connectScreen == null)
        {
            connectScreen = connectScene.Instantiate() as ConnectScreen;
            uiCanvasLayer.AddChild(connectScreen);

            connectScreen.ConnectButtonPressed += OnConnectButtonPressed;
        }


        // var res = await ToSignal(connectScreen, ConnectScreen.SignalName.ConnectButtonPressed);

        // 

        // connectScreen.Hide();
        // connectScreen.QueueFree();

        // GD.Print(res.GetValue(1));

        // var loginScreen = loginScreenScene.Instantiate() as Control;
        // uiCanvasLayer.AddChild(loginScreen);

        // var map = mapScene.Instantiate() as Map;
        // AddChild(map);
    }

    private async void OnConnectButtonPressed(string address, int port)
    {
        if (connectScreen == null)
        {
            GD.Print("Connect screen is null");
            return;
        }

        connectScreen.ShowError("Connecting to server...");

        if (!networkManager.StartClient(address, port))
        {
            connectScreen.ShowError("Could not connect to server");
            StartClient();
            return;
        }

        var connected = await ToSignal(networkManager, NetworkManager.SignalName.ClientConnected);
        if (!(bool)connected[0])
        {
            connectScreen.ShowError("Could not connect to server");
            StartClient();
            return;
        }

        connectScreen.ShowError("Connected to server");

        connectScreen.Hide();
        connectScreen.QueueFree();
        connectScreen = null;
    }
}
