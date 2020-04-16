using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Unit))]
public abstract class Status : MonoBehaviour, HasVariables
{
    public abstract string Name { get; }

    public virtual List<Variable> Variables
    {
        get { return Utility.List<Variable>(new ReadOnlyVariable("Is " + Name, true)); }
    }

    public Unit Unit { get { return GetComponent<Unit>(); } }
}
