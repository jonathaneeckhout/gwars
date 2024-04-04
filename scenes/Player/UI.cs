using Godot;
using System;

public partial class UI : CanvasLayer
{
    public bool AboveUI { get; set; } = false;
    private Control aboveUIChecker = null;

    public override void _Ready()
    {
        aboveUIChecker = GetNode<Control>("%AboveUIChecker");
        aboveUIChecker.MouseEntered += OnMouseEntered;
        aboveUIChecker.MouseExited += OnMouseExited;
    }
    private void OnMouseEntered()
    {
        AboveUI = false;
    }
    private void OnMouseExited()
    {
        AboveUI = true;
    }
}
