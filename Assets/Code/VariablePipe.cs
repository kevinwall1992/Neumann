using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VariablePipe
{
    public string PrimaryVariableName { get; set; } = null;
    public string SecondaryVariableName { get; set; } = null;

    public PipeFunction Function { get; set; } = null;

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

    public Variable GetPrimaryVariable(Unit unit)
    {
        Variable primary_variable = Variable.Find(unit, PrimaryVariableName);
        if (primary_variable == null)
            return null;

        return primary_variable;
    }

    public Variable GetSecondaryVariable(Unit unit)
    {
        Variable secondary_variable = Variable.Find(unit, SecondaryVariableName);
        if (secondary_variable == null)
            return null;

        return secondary_variable;
    }

    public object Read(Unit unit)
    {
        Variable primary_variable = GetPrimaryVariable(unit);
        if (primary_variable == null)
            return null;

        Variable secondary_variable = GetSecondaryVariable(unit);
        if(SecondaryVariableName != null)
            secondary_variable = Variable.Find(unit, SecondaryVariableName);

        if (Function != null)
            return Function(primary_variable.Read(), secondary_variable != null ? 
                                                     secondary_variable.Read() : 
                                                     null);
        else
            return primary_variable.Read();
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

        if (Function != null)
            primary_variable.Write(Function(value, secondary_variable != null ? 
                                                   secondary_variable.Read() : 
                                                   null));
        else
            primary_variable.Write(value);
    }


    public delegate object PipeFunction(object a, object b);

    public static Dictionary<string, PipeFunction> Functions = new Dictionary<string, PipeFunction>();
    
    static VariablePipe()
    {
        //Unary
        Functions["2x"] = Convert((a, b) => a * 2);
        Functions["10x"] = Convert((a, b) => a * 10);
        Functions["Sqrt"] = Convert((a, b) => Mathf.Sqrt(a));
        Functions["^2"] = Convert((a, b) => Mathf.Pow(a, 2));
        Functions["^y"] = Convert((a, b) => Mathf.Pow(a, b));
        Functions["log"] = (a, b) => b != null ? 
                              Mathf.Log((float)a, (float)b) : 
                              Mathf.Log((float)a);
        Functions["log10"] = Convert((a, b) => Mathf.Log(a));
        Functions["Not"] = Convert((a, b) => !a);

        //Binary
        Functions["+"] = Convert((a, b) => a + b);
        Functions["-"] = Convert((a, b) => a - b);
        Functions["x"] = Convert((a, b) => a * b);
        Functions["/"] = Convert((a, b) => a / b);
        Functions[">"] = Convert((a, b) => a > b);
        Functions["<"] = Convert((a, b) => a < b);
        Functions["And"] = Convert((a, b) => a && b);
        Functions["Or"] = Convert((a, b) => a || b);
        Functions["="] = (a, b) => a.Equals(b);
        Functions["Not ="] = (a, b) => a.Equals(b);
        Functions["Mod"] = (a, b) => (int)a % (int)b;
    }

    static PipeFunction Convert(System.Func<float, float, float> function)
    {
        return (a, b) => function((float)a, b == null ? 0 : (float)b);
    }

    static PipeFunction Convert(System.Func<float, float, bool> function)
    {
        return (a, b) => (a is float && b is float ? function((float)a, (float)b) : false);
    }

    static PipeFunction Convert(System.Func<bool, bool, bool> function)
    {
        return (a, b) => (a is bool && b is bool ? function((bool)a, (bool)b) : false);
    }
}


