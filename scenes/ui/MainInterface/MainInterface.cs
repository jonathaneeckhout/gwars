using Godot;
using System;

public partial class MainInterface : Control
{

    public Player Player { get; set; } = null;
    public Map Map { get; set; } = null;
    public MaterialComponent MaterialComponent
    {
        get { return materialComponent; }
        set
        {
            materialComponent = value;
            if (Player.IsOwnPlayer())
            {
                materialComponent.MaterialChanged += OnMaterialChanged;

                goldValue.Text = materialComponent.Gold.ToString();
                foodValue.Text = materialComponent.Food.ToString();
            }
        }
    }
    public DebugMenu DebugMenu { get; set; } = null;
    private MaterialComponent materialComponent = null;
    private Label goldValue = null;
    private Label foodValue = null;
    private Button buildingsButton = null;
    private Button debugButton = null;
    private Button townhallButton = null;

    private GridContainer buildingsContainer = null;

    private PackedScene buildingPreviewScene = GD.Load<PackedScene>("res://scenes/ui/previews/BuildingPreview/BuildingPreview.tscn");

    private BuildingPreview buildingPreview = null;

    public override void _Ready()
    {
        goldValue = GetNode<Label>("%GoldValue");
        foodValue = GetNode<Label>("%FoodValue");

        buildingsButton = GetNode<Button>("%BuildingsButton");
        buildingsButton.Pressed += OnBuildingsButtonPressed;

        debugButton = GetNode<Button>("%DebugButton");
        debugButton.Pressed += () => DebugMenu.Visible = !DebugMenu.Visible;

        townhallButton = GetNode<Button>("%TownhallButton");
        townhallButton.Pressed += () => ShowBuildingPreview("Townhall");

        buildingsContainer = GetNode<GridContainer>("%BuildingsContainer");
    }

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("left_click"))
        {
            HandleLeftClick();
        }
        else if (Input.IsActionJustPressed("right_click"))
        {
            HandleRightClick();
        }
    }

    private void HandleLeftClick()
    {
        if (buildingPreview != null)
        {
            buildingPreview.QueueFree();
            buildingPreview = null;
        }
    }

    private void HandleRightClick()
    {
        if (buildingPreview != null)
        {
            Map.RpcId(1, Map.MethodName.PlaceConstructionRPC, buildingPreview.BuildingName, Player.GetGlobalMousePosition());
            buildingPreview.QueueFree();
            buildingPreview = null;
        }
    }

    private void HideAllContainers()
    {
        buildingsContainer.Visible = false;
    }

    private void ShowBuildingPreview(string buildingName)
    {
        if (buildingPreview != null)
        {
            buildingPreview.QueueFree();
            buildingPreview = null;
        }

        buildingPreview = (BuildingPreview)buildingPreviewScene.Instantiate();
        buildingPreview.Name = "BuildingPreview";
        buildingPreview.Position = Player.GetGlobalMousePosition();
        Map.AddChild(buildingPreview);
        // This is needed to be done after the add to make sure that the panel is fetched in the ready call
        buildingPreview.BuildingName = buildingName;
    }

    private void OnMaterialChanged(uint gold, uint food)
    {
        goldValue.Text = gold.ToString();
        foodValue.Text = food.ToString();
    }

    private void OnBuildingsButtonPressed()
    {
        HideAllContainers();

        buildingsContainer.Visible = true;
    }
}
