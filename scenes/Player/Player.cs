using Godot;
using System;

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
    private MainInterface mainInterface = null;

    public override void _Ready()
    {
        playerCamera = GetNode<Camera2D>("%PlayerCamera");

        ui = GetNode<CanvasLayer>("%UI");

        unitSelectionComponent = GetNode<UnitSelectionComponent>("%UnitSelectionComponent");

        inputComponent = GetNode<InputComponent>("%InputComponent");

        groupComponent = GetNode<GroupComponent>("%GroupComponent");
        groupComponent.Map = Map;

        materialComponent = GetNode<MaterialComponent>("%MaterialComponent");

        if (!IsOwnPlayer())
        {
            SetProcessInput(false);

            unitSelectionComponent.QueueFree();
            unitSelectionComponent = null;

            inputComponent.QueueFree();
            inputComponent = null;

            playerCamera.QueueFree();
            playerCamera = null;

            ui.QueueFree();
            ui = null;

            return;
        }
        else
        {
            groupComponent.UnitSelectionComponent = unitSelectionComponent;

            inputComponent.UnitSelectionComponent = unitSelectionComponent;
            inputComponent.GroupComponent = groupComponent;
            inputComponent.Map = Map;
            inputComponent.Player = this;

            debugMenu = GetNode<DebugMenu>("%DebugMenu");
            debugMenu.Player = this;
            debugMenu.Map = Map;

            mainInterface = GetNode<MainInterface>("%MainInterface");
            mainInterface.DebugMenu = debugMenu;
            mainInterface.Player = this;
            mainInterface.Map = Map;
            mainInterface.MaterialComponent = materialComponent;
            mainInterface.UnitSelectionComponent = unitSelectionComponent;
        }
    }

    public bool IsOwnPlayer()
    {
        return !Multiplayer.IsServer() && Multiplayer.GetUniqueId() == PeerID;
    }

    public bool AddMaterial(string materialType, uint amount)
    {
        switch (materialType)
        {
            case "Gold":
                materialComponent.Gold += amount;
                return true;
            case "Food":
                materialComponent.Food += amount;
                return true;
            default:
                return false;
        }
    }

    public bool HasMaterials(uint gold, uint food)
    {
        return materialComponent.Gold >= gold && materialComponent.Food >= food;
    }

    public bool RemoveMaterials(uint gold, uint food)
    {
        if (HasMaterials(gold, food))
        {
            materialComponent.Gold -= gold;
            materialComponent.Food -= food;
            return true;
        }
        return false;
    }
}
