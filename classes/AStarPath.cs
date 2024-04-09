using Godot;
using Godot.Collections;
using System;

public partial class AStarPath : Node
{
    public const float ArrivalDistance = 8.0f;

    public Array<Vector2> Path { get; set; } = new Array<Vector2>();

    public bool IsNavigationFinished()
    {
        return Path.Count <= 0;
    }


    public Vector2 GetNextNavigationPoint(Vector2 currentPosition)
    {
        if (IsNavigationFinished())
        {
            return currentPosition;
        }

        Vector2 nextPoint = Path[0];
        if (currentPosition.DistanceTo(nextPoint) < ArrivalDistance)
        {
            Path.RemoveAt(0);
        }

        return nextPoint;
    }
}

