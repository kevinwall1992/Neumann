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
public class AgainOperation : Operation
{
    int repetition_count = 0;

    public Operation Target { get; set; }
    public Variable Repetitions { get; set; }

    public AgainOperation(Operation target = null, Variable repetitions = null)
    {
        Target = target;
        Repetitions = repetitions;
    }

    public bool Repeat()
    {
        object repetitions = Repetitions.Read();

        if (repetitions is bool)
            return (bool)repetitions;
        else if (repetitions is int || repetitions is float)
        {
            if (repetition_count >= (float)repetitions)
            {
                Reset();
                return false;
            }

            repetition_count++;
            return true;
        }
        else
            return false;
    }

    public void Reset()
    {
        repetition_count = 0;
    }


    public virtual Style.Operation Style
    { get { return Scene.Main.Style.AgainOperation; } }
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
