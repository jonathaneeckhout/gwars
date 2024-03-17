using Godot;
using System;

public partial class Main : Node2D
{
    private const string server_address = "127.0.0.1";
    private const int server_port = 9876;

    private NetworkManager networkManager;
    private Control networkControl;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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
        GetWindow().Title = "GWars Server";

        networkControl.Hide();

        networkManager.StartServer(server_port);

    }

    // Called when the client button is pressed
    private void OnClientButtonPressed()
    {
        GD.Print("Client button pressed");
        GetWindow().Title = "GWars Client";

        networkControl.Hide();

        networkManager.StartClient(server_address, server_port);

    }
}
