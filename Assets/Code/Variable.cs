using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Variable
{
    public abstract string Name { get; set; }
    public virtual bool IsReadOnly { get { return true; } }

    public abstract object Read();
    public virtual void Write(object value) { }

    public override bool Equals(object obj)
    {
        Variable other = obj as Variable;
        if (other == null)
            return false;

        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return 1;
    }


    //Visualization
    public Sprite Sprite { get; set; }
    public Color Color { get; set; }
    public string Units { get; set; }
}

public class ConstVariable<T> : Variable
{
    public override string Name { get; set; }
    public T Value { get; set; }

    public ConstVariable(string name, T value = default(T))
    {
        Name = name;
        Value = value;
    }

    public override object Read()
    {
        return Value;
    }
}

public class WritableVariable : Variable
{
    public override string Name { get; set; }
    public object Value { get; set; }
    public override bool IsReadOnly { get { return false; } }

    public WritableVariable(string name, object value = null)
    {
        Name = name;

        if (value != null)
            Value = value;
        else
            Value = new Int32();
    }

    public override object Read()
    {
        return Value;
    }

    public override void Write(object value)
    {
        Value = value;
    }
}

public class FunctionVariable : Variable
{
    System.Func<object> function;

    public override string Name { get; set; }

    public FunctionVariable(string name, System.Func<object> function_)
    {
        Name = name;
        function = function_;
    }

    public override object Read() { return function(); }
}


public static class VariableExtensions
{
    public static Variable Stylize(this Variable variable, Sprite sprite, Color color, string units)
    {
        variable.Sprite = sprite;
        variable.Color = color;
        variable.Units = units;

        return variable;
    }

    public static Variable Stylize(this Variable variable, Style.Variable variable_style)
    {
        return variable.Stylize(variable_style.Sprite, 
                                variable_style.Color, 
                                variable_style.Units);
    }
}

