using Godot;
using System;
using System.Diagnostics;

public partial class Player : Node2D
{
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
    public Map Map { get; set; } = null;
    public bool AboveUI { get; set; } = false;
    private string username = "Player";
    private Camera2D playerCamera = null;
    private CanvasLayer ui = null;
    private UnitSelectionComponent unitSelectionComponent = null;
    private InputComponent inputComponent = null;
    private GroupComponent groupComponent = null;

    private DebugMenu debugMenu = null;

    public override void _Ready()
    {
        playerCamera = GetNode<Camera2D>("%PlayerCamera");

        ui = GetNode<CanvasLayer>("%UI");

        unitSelectionComponent = GetNode<UnitSelectionComponent>("%UnitSelectionComponent");

        groupComponent = GetNode<GroupComponent>("%GroupComponent");
        groupComponent.UnitSelectionComponent = unitSelectionComponent;

        inputComponent = GetNode<InputComponent>("%InputComponent");
        inputComponent.UnitSelectionComponent = unitSelectionComponent;
        inputComponent.GroupComponent = groupComponent;

        debugMenu = GetNode<DebugMenu>("%DebugMenu");
        debugMenu.Player = this;
        debugMenu.Map = Map;

        // If we are the server or other players, we don't need to process input.
        if (Multiplayer.IsServer() || PeerID != Multiplayer.GetUniqueId())
        {
            SetProcessInput(false);
            playerCamera.QueueFree();
            ui.QueueFree();

            return;
        }
    }
}
