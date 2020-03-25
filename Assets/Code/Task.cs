using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Task : Operation
{
    public Unit Unit { get; set; }

    public Target Target { get { return Target.Convert(Input.Read(Unit)); } }
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
