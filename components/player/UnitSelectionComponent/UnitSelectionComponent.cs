using Godot;
using Godot.Collections;
using System;

public partial class UnitSelectionComponent : Node2D
{
    [Signal]
    public delegate void UnitsSelectedEventHandler(Array<Unit> units);

    [Signal]
    public delegate void UnitsDeselectedEventHandler(Array<Unit> units);

    [Export]
    public Array<Unit> selectedUnits = new();

    [Export]
    public bool Enabled
    {
        get
        {
            return enabled;
        }
        set
        {
            enabled = value;
            SetPhysicsProcess(enabled);
            SetProcessInput(enabled);

            selectionPanel?.Hide();

            dragging = false;
        }
    }
    public UI UI { get; set; } = null;
    private Panel selectionPanel = null;

    private bool enabled = true;

    private bool dragging = false;
    private Vector2 startPoint = Vector2.Zero;


    public override void _Ready()
    {
        selectionPanel = GetNode<Panel>("SelectionPanel");
    }

    public override void _Process(double delta)
    {
        if (dragging)
        {
            DrawSelectionPanel();
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (UI != null && UI.AboveUI)
        {
            return;
        }

        // Hide the settings menu if visible.
        if (Input.IsActionJustPressed("left_click"))
        {
            OnLeftClicked();
        }
        else if (Input.IsActionJustReleased("left_click"))
        {
            OnLeftReleased();
        }
    }

    private void OnLeftClicked()
    {
        dragging = true;
        startPoint = GetGlobalMousePosition();
        selectionPanel.Show();
    }

    private void OnLeftReleased()
    {
        Vector2 endPoint = GetGlobalMousePosition();
        float height = Mathf.Abs(endPoint.Y - startPoint.Y);
        float width = Mathf.Abs(endPoint.X - startPoint.X);

        dragging = false;

        if (selectedUnits.Count > 0)
        {
            EmitSignal(SignalName.UnitsDeselected, selectedUnits);
            selectedUnits.Clear();
        }

        RectangleShape2D selectRectangle = new()
        {
            Size = new Vector2(width, height)
        };
        PhysicsDirectSpaceState2D space = GetWorld2D().DirectSpaceState;
        Vector2 topCorner = CalculateTopCorner(startPoint, endPoint);
        PhysicsShapeQueryParameters2D query = new()
        {
            CollideWithAreas = false,
            CollideWithBodies = true,
            CollisionMask = 1,
            Shape = selectRectangle,
            Transform = new Transform2D(0, new Vector2(topCorner.X + (width / 2), topCorner.Y + (height / 2)))
        };
        Array<Dictionary> selected = space.IntersectShape(query);
        foreach (Dictionary res in selected)
        {
            Node node = res["collider"].As<Node>();
            if (node is Unit unit)
            {
                selectedUnits.Add(unit);
            }
        }

        if (selectedUnits.Count > 0)
        {
            EmitSignal(SignalName.UnitsSelected, selectedUnits);
        }

        selectionPanel.Hide();
    }

    private void DrawSelectionPanel()
    {
        Vector2 currentMousePostion = GetGlobalMousePosition();
        float height = Mathf.Abs(currentMousePostion.Y - startPoint.Y);
        float width = Mathf.Abs(currentMousePostion.X - startPoint.X);
        Vector2 size = new(width, height);

        selectionPanel.Size = size;

        selectionPanel.Position = CalculateTopCorner(currentMousePostion, startPoint);
    }

    private static Vector2 CalculateTopCorner(Vector2 start, Vector2 end)
    {
        Vector2 topCorner = new();

        if (end.X > start.X)
        {
            topCorner.X = start.X;
        }
        else
        {
            topCorner.X = end.X;
        }

        if (end.Y > start.Y)
        {
            topCorner.Y = start.Y;
        }
        else
        {
            topCorner.Y = end.Y;
        }
        return topCorner;
    }
}