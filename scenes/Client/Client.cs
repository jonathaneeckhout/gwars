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

    public Player Player { get; set; } = null;
}
