using Godot;
using Godot.Collections;
using System;

public partial class GroupComponent : Node
{
    public Map Map { get; set; } = null;

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

    public Vector2 GetAveragePosition()
    {
        Vector2 averagePosition = Vector2.Zero;
        if (Members.Count == 0)
        {
            return averagePosition;
        }

        foreach (Unit unit in Members)
        {
            averagePosition += unit.Position;
        }

        return averagePosition / Members.Count;
    }

    private void OnUnitsSelected(Array<Unit> units)
    {
        Array<string> unitNames = new Array<string>();
        foreach (Unit unit in units)
        {
            unitNames.Add(unit.Name);
            unit.SetSelected(true);
        }

        Members = units.Duplicate();

        RpcId(1, MethodName.SelectUnitsRPC, unitNames);

    }

    private void OnUnitsDeselected(Array<Unit> units)
    {
        foreach (Unit unit in units)
        {
            unit.SetSelected(false);
        }

        Members.Clear();

        RpcId(1, MethodName.DeselectUnitsRPC, null);
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void SelectUnitsRPC(Array<string> units)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        foreach (string unitName in units)
        {
            Unit unit = Map.GetUnit(unitName);
            if (unit != null)
            {
                Members.Add(unit);
            }
        }
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

        //TODO: check if player is logged in


        foreach (Unit unit in Members)
        {
            unit.MoveTo(position);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void GatherMaterialRPC(string materialName, string storageName)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        //TODO: check if player is logged in

        Material material = Map.GetMaterial(materialName);
        if (material == null)
        {
            GD.Print("Material not found");
            return;
        }

        Unit storage = Map.GetUnit(storageName);
        if (storage == null)
        {
            GD.Print("Storage not found");
            return;
        }

        foreach (Unit unit in Members)
        {
            unit.GatherMaterial(material, storage);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void AttackGroupRPC(string targetName)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        //TODO: check if player is logged in

        Unit target = Map.GetUnit(targetName);
        if (target == null)
        {
            GD.Print("Target not found");
            return;
        }

        foreach (Unit unit in Members)
        {
            unit.AttackUnit(target);
        }
    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void RepairGroupRPC(string targetName)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        //TODO: check if player is logged in

        Unit target = Map.GetUnit(targetName);
        if (target == null)
        {
            GD.Print("Target not found");
            return;
        }

        //Check if target is repairable
        if (!target.IsRepairable)
        {
            GD.Print("Target is not a repairable unit");
            return;
        }

        foreach (Unit unit in Members)
        {
            unit.RepairTarget(target);
        }

    }

    [Rpc(MultiplayerApi.RpcMode.AnyPeer)]
    public void TrainUnitGroupRPC(string trainingsCenter, string unitType)
    {
        if (!Multiplayer.IsServer())
        {
            return;
        }

        //TODO: check if player is logged in

        Unit unit = Map.GetUnit(trainingsCenter);
        if (unit == null)
        {
            GD.Print("Training center not found");
            return;
        }

        //Check if unit is a training center
        if (!unit.IsTrainingCenter)
        {
            GD.Print("Unit is not a training center");
            return;
        }

        unit.TrainUnit(unitType);
    }
}
