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

    public virtual void Execute(Unit unit)
    {
        int next_index = unit.Program.IndexOf(this) + 1;
        if (next_index < unit.Program.Count)
            unit.Program.Next = unit.Program[next_index];
        else
            unit.Program.Next = null;
    }

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

    public override void Execute(Unit unit)
    {
        bool? condition_value = Condition.Read<bool?>(unit);

        if (Goto != null &&
            unit.Program.Contains(Goto) &&
            (condition_value == null || condition_value.Value))
        {
            unit.Program.Next = Goto;
            return;
        }

        base.Execute(unit);
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

    public override void Execute(Unit unit)
    {
        bool? condition_value = Condition.Read<bool?>(unit);

        if (condition_value != null && condition_value.Value)
            return;

        base.Execute(unit);
    }

    public override Operation Instantiate() { return new YieldOperation(); }

    public override Style.Operation Style
    { get { return Scene.Main.Style.YieldOperation; } }
}


//Stops the current Task
[System.Serializable]
public class InterruptOperation : Operation
{
    public override void Execute(Unit unit)
    {
        unit.Task = null;

        base.Execute(unit);
    }

    public override Operation Instantiate() { return new InterruptOperation(); }

    public override Style.Operation Style
    { get { return Scene.Main.Style.InterruptOperation; } }
}


//Writes Input to Output
public class WriteOperation : Operation
{
    public override bool TakesInput { get { return true; } }
    public override bool HasOutput { get { return true; } }

    public override void Execute(Unit unit)
    {
        if (Input.IsConnected(unit) && Output.IsConnected(unit))
            Output.Write(unit, Input.Read(unit));

        base.Execute(unit);
    }

    public override Operation Instantiate() { return new WriteOperation(); }

    public override Style.Operation Style
    { get { return Scene.Main.Style.WriteOperation; } }
}
