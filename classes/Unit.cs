using Godot;
using System;

public partial class Unit : CharacterBody2D
{

    public Player Player { get; set; } = null;
    protected Vector2 targetPosition = Vector2.Zero;

    protected Panel selectionPanel = null;


    public override void _Ready()
    {
        targetPosition = Position;
        selectionPanel = GetNodeOrNull<Panel>("%SelectedPanel");

        if (!Multiplayer.IsServer())
        {
            SetPhysicsProcess(false);
        }
    }

    public virtual void SetSelected(bool value)
    {
        if (selectionPanel != null)
        {
            selectionPanel.Visible = value;
        }
    }

    public virtual void MoveTo(Vector2 position) { }

    public virtual void GatherMaterial(Material material, Unit Storage) { }

    public virtual void StoreMaterial(string materialType, uint amount) { }
}
