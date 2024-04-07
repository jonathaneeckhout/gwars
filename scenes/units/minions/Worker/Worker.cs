using Godot;
using Godot.Collections;
using System;

public partial class Worker : Unit
{
    public float Speed = 300.0f;
    public uint MaxMaterialsInBag { get; set; } = 5;
    public float GatherSpeed = 1.0f;
    public uint GatherEfficiency = 1;
    public float RepairSpeed = 1.0f;
    public uint RepairEfficiency = 10;
    private Area2D interactArea = null;
    private Timer attackDelayTimer = null;
    private Timer gatherTimer = null;
    private Timer repairTimer = null;
    private Material materialTarget = null;
    private Unit attackTarget = null;
    private Unit storageTarget = null;
    private Unit repairTarget = null;

    private Array<Node> bodiesInInteractRange = new Array<Node>();

    [Export]
    private string materialTypeInBag = "";
    [Export]
    private uint amountOfMaterialInBag = 0;


    public override void _Ready()
    {
        base._Ready();

        interactArea = GetNode<Area2D>("%InteractArea");
        interactArea.BodyEntered += OnInteractAreaBodyEntered;
        interactArea.BodyExited += OnInteractAreaBodyExited;

        attackDelayTimer = new Timer
        {
            Name = "AttackDelayTimer",
            OneShot = true,
            WaitTime = AttackSpeed,
            Autostart = false
        };
        AddChild(attackDelayTimer);

        gatherTimer = new Timer
        {
            Name = "GatherTimer",
            OneShot = true,
            WaitTime = GatherSpeed,
            Autostart = false
        };
        gatherTimer.Timeout += OnGatherTimerTimeout;
        AddChild(gatherTimer);

        repairTimer = new Timer
        {
            Name = "RepairTimer",
            OneShot = true,
            WaitTime = RepairSpeed,
            Autostart = false
        };
        repairTimer.Timeout += OnRepairTimerTimeout;
        AddChild(repairTimer);
    }


    public override void _PhysicsProcess(double delta)
    {
        if (!HandleAttacking())
        {
            if (!HandleGathering())
            {
                if (!HandleRepairing())
                {
                    if (!HandleMoveTo())
                    {
                        Velocity = Vector2.Zero;
                    }
                }
            }
        }

        MoveAndSlide();
    }

    private bool HandleAttacking()
    {
        if (attackTarget != null)
        {
            if (!IsInstanceValid(attackTarget))
            {
                ResetInputs();
                return false;
            }

            if (!bodiesInInteractRange.Contains(attackTarget))
            {
                Velocity = Position.DirectionTo(attackTarget.Position) * Speed;
            }
            else
            {
                Velocity = Vector2.Zero;

                if (attackDelayTimer.IsStopped())
                {
                    attackTarget.Hurt(AttackPower);
                    attackDelayTimer.Start(AttackSpeed);
                }
            }

            return true;
        }

        return false;
    }

    private bool HandleGathering()
    {
        if (materialTarget != null && amountOfMaterialInBag < MaxMaterialsInBag)
        {
            if (!IsInstanceValid(materialTarget))
            {
                ResetInputs();
                return false;
            }

            if (!bodiesInInteractRange.Contains(materialTarget))
            {
                Velocity = Position.DirectionTo(materialTarget.Position) * Speed;
            }
            else
            {
                Velocity = Vector2.Zero;

                if (gatherTimer.IsStopped())
                {
                    gatherTimer.Start(GatherSpeed);
                }
            }

            return true;
        }
        else if (storageTarget != null)
        {
            if (!IsInstanceValid(storageTarget))
            {
                ResetInputs();
                return false;
            }

            if (!bodiesInInteractRange.Contains(storageTarget))
            {
                Velocity = Position.DirectionTo(storageTarget.Position) * Speed;
            }
            else
            {
                Velocity = Vector2.Zero;

                if (storageTarget.StoreMaterial(materialTypeInBag, amountOfMaterialInBag))
                {
                    amountOfMaterialInBag = 0;
                    materialTypeInBag = "";
                }
            }

            return true;
        }

        return false;
    }

    private bool HandleRepairing()
    {
        if (repairTarget != null)
        {
            if (!IsInstanceValid(repairTarget))
            {
                ResetInputs();
                return false;
            }

            if (!bodiesInInteractRange.Contains(repairTarget))
            {
                Velocity = Position.DirectionTo(repairTarget.Position) * Speed;
            }
            else
            {
                Velocity = Vector2.Zero;

                if (repairTimer.IsStopped())
                {
                    repairTimer.Start(RepairSpeed);
                }
            }

            return true;
        }

        return false;
    }

    private bool HandleMoveTo()
    {
        if (Position.DistanceTo(targetPosition) > 8.0f)
        {
            Velocity = Position.DirectionTo(targetPosition) * Speed;
            return true;
        }

        return false;
    }

    public override void MoveTo(Vector2 position)
    {
        ResetInputs();

        targetPosition = position;
    }

    public override void AttackUnit(Unit target)
    {
        ResetInputs();

        targetPosition = target.Position;
        attackTarget = target;
    }

    public override void GatherMaterial(Material material, Unit Storage)
    {
        ResetInputs();

        targetPosition = material.Position;
        materialTarget = material;
        storageTarget = Storage;

    }

    public override void RepairTarget(Unit target)
    {
        ResetInputs();

        targetPosition = target.Position;
        repairTarget = target;
    }

    private void ResetInputs()
    {
        targetPosition = Position;
        attackTarget = null;
        materialTarget = null;
        storageTarget = null;
        repairTarget = null;
        gatherTimer.Stop();
    }

    private void OnInteractAreaBodyEntered(Node body)
    {
        if (!bodiesInInteractRange.Contains(body))
        {
            bodiesInInteractRange.Add(body);
        }
    }

    private void OnInteractAreaBodyExited(Node body)
    {
        if (bodiesInInteractRange.Contains(body))
        {
            bodiesInInteractRange.Remove(body);
        }
    }
    private void OnGatherTimerTimeout()
    {
        if (materialTarget == null)
        {
            return;
        }

        if (!IsInstanceValid(materialTarget))
        {
            ResetInputs();
            return;
        }

        if (!bodiesInInteractRange.Contains(materialTarget))
        {
            return;
        }

        if (amountOfMaterialInBag >= MaxMaterialsInBag)
        {
            return;
        }

        if (materialTypeInBag == materialTarget.MaterialType)
        {
            amountOfMaterialInBag = Math.Min(MaxMaterialsInBag, amountOfMaterialInBag + GatherEfficiency);
        }
        else
        {
            materialTypeInBag = materialTarget.MaterialType;
            amountOfMaterialInBag = GatherEfficiency;
        }
    }


    private void OnRepairTimerTimeout()
    {
        if (repairTarget == null)
        {
            ResetInputs();
            return;
        }

        if (!IsInstanceValid(repairTarget))
        {
            ResetInputs();
            return;
        }

        if (!bodiesInInteractRange.Contains(repairTarget))
        {
            return;
        }

        if (!repairTarget.GetRepaired(RepairEfficiency))
        {
            ResetInputs();
            return;
        }
    }
}
