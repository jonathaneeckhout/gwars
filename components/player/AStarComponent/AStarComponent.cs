using Godot;
using Godot.Collections;
using Godot.NativeInterop;
using System;
using System.Collections;

public partial class AStarComponent : Node
{
    public static readonly Vector2I[] Directions = {
        Vector2I.Right,
        new Vector2I(1, 1),
        Vector2I.Up,
        new Vector2I(-1, 1),
        Vector2I.Left,
        new Vector2I(-1, -1),
        Vector2I.Down,
        new Vector2I(1, -1)
         };

    public TileMap NavigationMap { get; set; } = null;
    private AStar2D aStar2D = null;
    public override void _Ready()
    {
        if (!Multiplayer.IsServer())
        {
            QueueFree();
            return;
        }

        aStar2D = new AStar2D();
    }


    public void CreatePathfindingPoints()
    {
        aStar2D.Clear();

        Array<Vector2I> usedCellPositions = NavigationMap.GetUsedCells(0);

        foreach (Vector2I cellPosition in usedCellPositions)
        {
            aStar2D.AddPoint(GetPoint(cellPosition), cellPosition);
        }

        foreach (Vector2I cellPosition in usedCellPositions)
        {
            ConnectCardinals(cellPosition);
        }
    }

    public AStarPath GetPath(Vector2 start, Vector2 end)
    {
        Vector2 mapStart = NavigationMap.LocalToMap(start);
        Vector2 mapEnd = NavigationMap.LocalToMap(end);

        if (!HasPoint(mapStart) || !HasPoint(mapEnd))
        {
            return null;
        }

        Vector2[] astarPath = aStar2D.GetPointPath(GetPoint(mapStart), GetPoint(mapEnd));

        AStarPath path = new AStarPath();
        foreach (Vector2 point in astarPath)
        {
            path.Path.Add(NavigationMap.MapToLocal((Vector2I)point));
        }

        return path;
    }


    private int GetPoint(Vector2 position)
    {
        int a = (int)position.X;
        int b = (int)position.Y;

        return SzudzikPaiImproved(a, b);
    }

    private bool HasPoint(Vector2 position)
    {
        return aStar2D.HasPoint(GetPoint(position));
    }

    private void ConnectCardinals(Vector2 pointPosition)
    {
        int center = GetPoint(pointPosition);

        foreach (Vector2I direction in Directions)
        {
            int cardinalPoint = GetPoint(pointPosition + NavigationMap.LocalToMap(direction));
            if (cardinalPoint != center && aStar2D.HasPoint(cardinalPoint))
            {
                aStar2D.ConnectPoints(center, cardinalPoint, true);
            }
        }
    }

    private int SzudzikPaiImproved(int x, int y)
    {
        int a = 0;
        int b = 0;

        if (x >= 0)
        {
            a = x * 2;
        }
        else
        {
            a = (x * -2) - 1;
        }

        if (y >= 0)
        {
            b = y * 2;
        }
        else
        {
            b = (y * -2) - 1;
        }

        int c = SzudzikPair(a, b);
        if (a >= 0 && b < 0 || b >= 0 && a < 0)
        {
            return -c - 1;
        }

        return c;
    }

    private int SzudzikPair(int a, int b)
    {
        if (a >= b)
        {
            return (a * a) + a + b;
        }
        else
        {
            return (b * b) + a;
        }
    }
}
