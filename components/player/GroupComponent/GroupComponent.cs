using Godot;
using Godot.Collections;
using System;

public partial class GroupComponent : Node
{
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
    [Export]
    public Array<Unit> Members { get; set; } = new Array<Unit>();


    void OnUnitsSelected(Array<Unit> units)
    {
        foreach (Unit unit in units)
        {
            unit.SetSelected(true);
        }

        RpcId(1, MethodName.SelectUnitsRPC, units);

    }

    void OnUnitsDeselected(Array<Unit> units)
    {
        foreach (Unit unit in units)
        {
            unit.SetSelected(false);
        }

        RpcId(1, MethodName.DeselectUnitsRPC, null);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void SelectUnitsRPC(Array<Unit> units)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        Members = units.Duplicate();
    }


    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void DeselectUnitsRPC()
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        Members.Clear();
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void MoveGroupRPC(Vector2 position)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }


        GD.Print("Moving group");
        GD.Print("Members count: " + Members.Count);

        foreach (Unit unit in Members)
        {
            GD.Print("Moving unit to " + position);
            unit.MoveTo(position);
        }
    }
}
