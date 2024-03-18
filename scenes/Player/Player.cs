using Godot;
using System;

public partial class Player : Node2D
{
    private PlayerInput playerInput = null;
    // Called when the node enters the scene tree for the first time.
    private Camera2D playerCamera = null;

    private string username = "Player";

    [Export]
    public string Username
    {
        get
        {
            return username;
        }
        set
        {
            username = value;
            Name = username;
        }
    }

    [Export]
    public long PeerID { get; set; } = -1;

    public override void _Ready()
    {
        // playerInput = GetNode<PlayerInput>("%PlayerInput");
        playerCamera = GetNode<Camera2D>("%PlayerCamera");

        // If we are the server or other players, we don't need to process input.
        if (Multiplayer.IsServer() || PeerID != Multiplayer.GetUniqueId())
        {
            SetProcessInput(false);
            playerCamera.QueueFree();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {

    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
        {
            switch (mouseEvent.ButtonIndex)
            {
                case MouseButton.Left:
                    GD.Print($"Left button was clicked at {mouseEvent.Position}");

                    break;
                case MouseButton.WheelUp:
                    GD.Print("Wheel up");
                    break;
            }
        }
    }
}
