using Godot;
using Godot.Collections;
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
    private VBoxContainer trainingContainer = null;
    private GridContainer trainingUnits = null;

    private PackedScene buildingPreviewScene = GD.Load<PackedScene>("res://scenes/ui/previews/BuildingPreview/BuildingPreview.tscn");
    private PackedScene trainUnitButtonScene = GD.Load<PackedScene>("res://scenes/ui/MainInterface/TrainUnitButton/TrainUnitButton.tscn");
    private BuildingPreview buildingPreview = null;

    private UnitSelectionComponent unitSelectionComponent = null;

    public UnitSelectionComponent UnitSelectionComponent
    {
        get
        {
            return unitSelectionComponent;
        }
        set
        {
            // You can add custom logic here
            if (value != null)
            {
                unitSelectionComponent = value;

                if (!Multiplayer.IsServer())
                {
                    unitSelectionComponent.UnitsSelected += OnUnitsSelected;
                    unitSelectionComponent.UnitsDeselected += OnUnitsDeselected;
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(value), "UnitSelectionComponent cannot be null");
            }
        }
    }

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

        trainingContainer = GetNode<VBoxContainer>("%TrainingsContainer");
        trainingUnits = GetNode<GridContainer>("%TrainingUnits");
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
        buildingsContainer.Hide();
        trainingContainer.Hide();
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

    private void ClearAllTrainableUnits()
    {
        foreach (Node child in trainingUnits.GetChildren())
        {
            child.QueueFree();
        }
    }

    private void LoadAllTrainableUnits(Unit unit)
    {
        ClearAllTrainableUnits();

        if (unit is Townhall)
        {
            foreach (string unitType in Townhall.Units.Keys)
            {
                TrainUnitButton button = (TrainUnitButton)trainUnitButtonScene.Instantiate();
                button.Text = unitType;
                button.GroupComponent = Player.GroupComponent;
                button.TrainingCenter = unit;
                button.UnitType = unitType;
                trainingUnits.AddChild(button);
            }
        }
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

    private void OnUnitsSelected(Array<string> units)
    {
        if (units.Count == 1)
        {
            Unit unit = Map.GetUnit(units[0]);
            // Check if the unit is owned by the player and if it is a townhall class
            if (unit != null && unit.PlayerName == Player.Username && unit.IsTrainingCenter)
            {
                LoadAllTrainableUnits(unit);
                trainingContainer.Visible = true;
            }
        }
    }

    private void OnUnitsDeselected(Array<string> units)
    {
        HideAllContainers();
    }
}
