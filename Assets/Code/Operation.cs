using UnityEngine;
using System.Collections;

public interface Operation : OperationTile.Stylish
{

}

[System.Serializable]
public class ChooseOperation : Operation
{
    public Operation Target { get; set; }
    public Variable Condition { get; set; }

    public ChooseOperation(Operation target = null, Variable condition = null)
    {
        Target = target;
        Condition = condition;
    }


    public virtual Style.Operation Style
    { get { return Scene.Main.Style.ChooseOperation; } }
}

[System.Serializable]
public class YieldOperation : Operation
{
    public Variable Seconds { get; set; }

    public YieldOperation(Variable seconds = null)
    {
        Seconds = seconds;
    }


    public virtual Style.Operation Style
    { get { return Scene.Main.Style.YieldOperation; } }
}
