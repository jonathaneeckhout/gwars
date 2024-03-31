using System;
using Godot;
using Godot.Collections;


public partial class InputComponent : Node2D
{
    public UnitSelectionComponent UnitSelectionComponent { get; set; } = null;
    public GroupComponent GroupComponent { get; set; } = null;

    public Map Map { get; set; } = null;
    public Player Player { get; set; } = null;

    private bool numSelected = false;

    public override void _Input(InputEvent @event)
    {
        if (Input.IsActionJustPressed("num1"))
        {
            numSelected = !numSelected;
            UnitSelectionComponent.Enabled = !numSelected;

            GD.Print("Num1 pressed");
        }
        else if (Input.IsActionJustPressed("right_click"))
        {
            HandleRightClick();
        }
    }

    private void HandleRightClick()
    {
        Node2D target = QueryTargetsUnderMouse();
        if (target != null)
        {
            GD.Print("Target: " + target.Name);
        }

        if (GroupComponent.Members.Count > 0)
        {
            if (target != null)
            {
                if (target is Material material)
                {
                    HandleGathering(material);
                }
                else if (target is Unit targetUnit)
                {
                    if (targetUnit.PlayerName == Player.Name)
                    {
                        if (targetUnit.IsRepairable)
                        {
                            GroupComponent.RpcId(1, GroupComponent.MethodName.RepairGroupRPC, target.Name);
                        }
                        else
                        {
                            GroupComponent.RpcId(1, GroupComponent.MethodName.MoveGroupRPC, target.GlobalPosition);
                        }
                    }
                    else
                    {
                        GroupComponent.RpcId(1, GroupComponent.MethodName.AttackGroupRPC, target.Name);
                    }
                }
                else
                {
                    GroupComponent.RpcId(1, GroupComponent.MethodName.MoveGroupRPC, target.GlobalPosition);
                }
            }
            else
            {
                GroupComponent.RpcId(1, GroupComponent.MethodName.MoveGroupRPC, GetGlobalMousePosition());
            }
        }
    }

    private void HandleGathering(Material material)
    {
        Vector2 averagePosition = GroupComponent.GetAveragePosition();

        Unit storage = Map.GetClosestStorage(Player.Name, averagePosition);
        if (storage != null)
        {
            GroupComponent.RpcId(1, GroupComponent.MethodName.GatherMaterialRPC, material.Name, storage.Name);
        }
        else
        {
            GD.Print("No storage found");
        }
    }

    private Node2D QueryTargetsUnderMouse()
    {
        Vector2 mousePosition = GetGlobalMousePosition();

        PhysicsDirectSpaceState2D space = GetWorld2D().DirectSpaceState;

        PhysicsPointQueryParameters2D pointParameters = new PhysicsPointQueryParameters2D();
        pointParameters.CollisionMask = 1;
        pointParameters.CollideWithAreas = false;
        pointParameters.CollideWithBodies = true;
        pointParameters.Position = mousePosition;

        Array<Dictionary> spaceStateResults = space.IntersectPoint(pointParameters, 1);

        if (spaceStateResults.Count > 0)
        {
            Dictionary spaceStateResult = (Godot.Collections.Dictionary)spaceStateResults[0];
            Node2D target = (Node2D)spaceStateResult["collider"];

            return target;
        }

        return null;
    }
}
