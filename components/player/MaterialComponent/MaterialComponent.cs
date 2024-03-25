using Godot;
using System;

public partial class MaterialComponent : Node
{
    [Signal]
    public delegate void MaterialChangedEventHandler(uint gold, uint food);

    [Export]
    public uint Gold
    {
        get { return gold; }
        set
        {
            gold = value;
            EmitSignal(SignalName.MaterialChanged, gold, food);
        }
    }
    [Export]
    public uint Food
    {
        get { return food; }
        set
        {
            food = value;
            EmitSignal(SignalName.MaterialChanged, gold, food);
        }
    }
    private uint gold = 0;
    private uint food = 0;

}
