using Godot;
using System;

public partial class Worker : Unit
{
    public const float Speed = 300.0f;

    private Panel selectionPanel = null;

    public override void _Ready()
    {
        selectionPanel = GetNode<Panel>("%SelectedPanel");
    }

    public override void SetSelected(bool value)
    {
        selectionPanel.Visible = value;
    }

}
