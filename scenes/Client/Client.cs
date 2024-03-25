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
    public override void _Ready()
    {
    }
}
