﻿using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Task : Operation
{
    protected Unit Unit { get; private set; }

    public Target Target
    {
        get
        {
            if (Unit == null)
                return null;

            return Target.Convert(Input.Read(Unit));
        }
    }
    public Target SpeculativeTarget
    {
        get
        {
            if (Target != null && 
                (OperationTileNode.Selected == null || 
                 OperationTileNode.Selected.OperationTile.Operation != this))
                return Target;

            return Scene.Main.World.Asteroid.GetWorldPositionPointedAt();
        }
    }

    public override bool TakesInput { get { return true; } }

    public abstract bool IsComplete { get; }

    public override void Execute(Unit unit)
    {
        if (unit.Task != null)
            return;

        unit.Task = this;
        Unit = unit;
        base.Execute(unit);
    }
}
