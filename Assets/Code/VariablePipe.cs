using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VariablePipe
{
    public string PrimaryVariableName { get; set; } = null;
    string SecondaryVariableName { get; set; } = null;

    public PipeFunction Function { get; set; } = Identity;

    public VariablePipe()
    {
        
    }

    public bool IsConnected(Unit unit)
    {
        if (PrimaryVariableName == null)
            return false;
        Variable primary_variable = Variable.Find(unit, PrimaryVariableName);
        if (primary_variable == null)
            return false;

        return true;
    }

    public object Read(Unit unit)
    {
        if (!IsConnected(unit))
            return null;
        
        Variable primary_variable = Variable.Find(unit, PrimaryVariableName);

        Variable secondary_variable = null;
        if(SecondaryVariableName != null)
            secondary_variable = Variable.Find(unit, SecondaryVariableName);

        return Function(primary_variable.Read(), secondary_variable != null ? secondary_variable.Read() : null);
    }

    public T Read<T>(Unit unit)
    {
        return (T)Read(unit);
    }

    public void Write(Unit unit, object value)
    {
        if (!IsConnected(unit))
            return;

        Variable primary_variable = Variable.Find(unit, PrimaryVariableName);

        Variable secondary_variable = null;
        if (SecondaryVariableName != null)
            secondary_variable = Variable.Find(unit, SecondaryVariableName);

        primary_variable.Write(Function(value, secondary_variable != null ? secondary_variable.Read() : null));
    }


    public delegate object PipeFunction(object a, object b);
    public static PipeFunction Identity = (a, b) => a;

    public static Dictionary<string, PipeFunction> Functions = new Dictionary<string, PipeFunction>();
    
    static VariablePipe()
    {
        //Unary
        Functions["Identity"] = Identity;
        Functions["Double"] = Convert((a, b) => a * 2);
        Functions["Not"] = Convert((a, b) => !a);
        Functions["IsNull"] = (a, b) => a == null;

        //Binary
        Functions["Add"] = Convert((a, b) => a + b);
        Functions["Subtract"] = Convert((a, b) => a - b);
        Functions["Multiply"] = Convert((a, b) => a * b);
        Functions["Divide"] = Convert((a, b) => a / b);
        Functions["Greater"] = Convert((a, b) => a > b);
        Functions["Less"] = Convert((a, b) => a < b);
        Functions["And"] = Convert((a, b) => a && b);
        Functions["Or"] = Convert((a, b) => a || b);
        Functions["Equals"] = (a, b) => a.Equals(b);
    }

    static PipeFunction Convert(System.Func<int, int, int> function)
    {
        return (a, b) => function((int)a, (int)b);
    }

    static PipeFunction Convert(System.Func<int, int, bool> function)
    {
        return (a, b) => (a is int && b is int ? function((int)a, (int)b) : false);
    }

    static PipeFunction Convert(System.Func<bool, bool, bool> function)
    {
        return (a, b) => (a is bool && b is bool ? function((bool)a, (bool)b) : false);
    }
}


