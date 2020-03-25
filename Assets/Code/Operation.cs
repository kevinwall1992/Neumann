using UnityEngine;
using System.Collections;

public abstract class Operation
{
    public VariablePipe Input { get; private set; } = new VariablePipe();
    public VariablePipe Output { get; private set; } = new VariablePipe();
    public Operation Goto { get; set; } = null;

    public virtual bool TakesInput { get { return false; } }
    public virtual bool HasOutput { get { return false; } }
    public virtual bool TakesGoto { get { return false; } }

    public abstract Operation Instantiate();

    public abstract Style.Operation Style { get; }
}

//Takes boolean input and Operation output
//If input is true, moves execution to Target
[System.Serializable]
public class ChooseOperation : Operation
{
    public VariablePipe Condition { get { return Input; } }

    public override bool TakesInput { get { return true; } }
    public override bool TakesGoto { get { return true; } }

    public ChooseOperation(Operation goto_ = null, string condition_variable_name = null)
    {
        Condition.PrimaryVariableName = condition_variable_name;
        Goto = goto_;
    }

    public override Operation Instantiate() { return new ChooseOperation(); }

    public override Style.Operation Style
    { get { return Scene.Main.Style.ChooseOperation; } }
}

//a) Given numeric input, waits that many seconds
//b) Given boolean input, waits until input is false
[System.Serializable]
public class YieldOperation : Operation
{
    public VariablePipe Condition { get { return Input; } }

    public override bool TakesInput { get { return true; } }

    public YieldOperation(string condition_variable_name = null)
    {
        Condition.PrimaryVariableName = condition_variable_name;
    }

    public override Operation Instantiate() { return new YieldOperation(); }

    public override Style.Operation Style
    { get { return Scene.Main.Style.YieldOperation; } }
}