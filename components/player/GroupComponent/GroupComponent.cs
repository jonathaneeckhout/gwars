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
                unitSelectionComponent.UnitsSelected += OnUnitsSelected;
                unitSelectionComponent.UnitsDeselected += OnUnitsDeselected;
            }
            else
            {
                throw new ArgumentNullException(nameof(value), "UnitSelectionComponent cannot be null");
            }
        }
    }
    public Array<Unit> Members { get; set; } = new Array<Unit>();


    void OnUnitsSelected(Array<Unit> units)
    {
        Members = units.Duplicate();
        foreach (Unit unit in Members)
        {
            unit.SetSelected(true);
        }
    }

    void OnUnitsDeselected(Array<Unit> units)
    {
        foreach (Unit unit in Members)
        {
            unit.SetSelected(false);
        }

        Members.Clear();
    }
}
