using Godot;
using System;

public partial class MaterialsMenu : Control
{
    public Player Player { get; set; } = null;
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
    private MaterialComponent materialComponent = null;
    private Label goldValue = null;
    private Label foodValue = null;

    public override void _Ready()
    {
        goldValue = GetNode<Label>("%GoldValue");
        foodValue = GetNode<Label>("%FoodValue");
    }

    private void OnMaterialChanged(uint gold, uint food)
    {
        goldValue.Text = gold.ToString();
        foodValue.Text = food.ToString();
    }
}
