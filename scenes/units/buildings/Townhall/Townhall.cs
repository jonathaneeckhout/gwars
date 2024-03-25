using Godot;
using System;

public partial class Townhall : Unit
{
    private Panel selectionPanel = null;

    public override void _Ready()
    {
        selectionPanel = GetNode<Panel>("%SelectedPanel");

        if (!Multiplayer.IsServer())
        {
            SetPhysicsProcess(false);
        }
    }

    public override void SetSelected(bool value)
    {
        selectionPanel.Visible = value;
    }

}
