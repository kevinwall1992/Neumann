using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class Task : Operation
{
    public abstract bool IsComplete { get; }


    public virtual Style.Operation Style
    { get { return new Style.Operation(); } }
}
