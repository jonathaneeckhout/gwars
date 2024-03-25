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
    private MaterialComponent materialComponent = null;

    private DebugMenu debugMenu = null;
    private MaterialsMenu materialsMenu = null;

    public override void _Ready()
    {
        playerCamera = GetNode<Camera2D>("%PlayerCamera");

        ui = GetNode<CanvasLayer>("%UI");

        unitSelectionComponent = GetNode<UnitSelectionComponent>("%UnitSelectionComponent");

        groupComponent = GetNode<GroupComponent>("%GroupComponent");
        groupComponent.Map = Map;
        groupComponent.UnitSelectionComponent = unitSelectionComponent;

        inputComponent = GetNode<InputComponent>("%InputComponent");
        inputComponent.UnitSelectionComponent = unitSelectionComponent;
        inputComponent.GroupComponent = groupComponent;

        materialComponent = GetNode<MaterialComponent>("%MaterialComponent");

        debugMenu = GetNode<DebugMenu>("%DebugMenu");
        debugMenu.Player = this;
        debugMenu.Map = Map;

        materialsMenu = GetNode<MaterialsMenu>("%MaterialsMenu");
        materialsMenu.Player = this;
        materialsMenu.MaterialComponent = materialComponent;

        // If we are the server or other players, we don't need to process input.
        if (!IsOwnPlayer())
        {
            SetProcessInput(false);
            playerCamera.QueueFree();
            ui.QueueFree();

            return;
        }
    }

    public bool IsOwnPlayer()
    {
        return !Multiplayer.IsServer() && Multiplayer.GetUniqueId() == PeerID;
    }
}
