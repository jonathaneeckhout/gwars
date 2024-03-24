using Godot;
using System;

public partial class Unit : CharacterBody2D
{
    public virtual void SetSelected(bool value) { }

    public virtual void MoveTo(Vector2 position) { }
}
