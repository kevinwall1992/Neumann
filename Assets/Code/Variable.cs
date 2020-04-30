using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Variable
{
    public Style.Variable Style
    {
        get
        {
            if (!Scene.Main.Style.Variables.ContainsKey(Name))
                return new Style.Variable(null, Color.white, "F1");

            return Scene.Main.Style.Variables[Name];
        }
    }

    public abstract string Name { get; set; }
    public virtual bool IsReadOnly { get { return true; } }

    public abstract object Read();
    public virtual void Write(object value) { }

    public T Read<T>(T default_value = default(T))
    {
        object value = Read();
        if (value is T)
            return (T)value;

        return default_value;
    }

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


    public static Variable Find(Unit unit, string name)
    {
        Predicate<Variable> match = (variable_) => (variable_.Name == name);

        Variable variable = unit.Variables.Find(match);
        if (variable != null)
            return variable;

        return Scene.Main.World.Variables.Find(match);
    }
}

public class ReadOnlyVariable : Variable
{
    public override string Name { get; set; }
    public object Value { get; set; }

    public ReadOnlyVariable(string name, object value = null)
    {
        Name = name;
        Value = value;
    }

    public override object Read()
    {
        return Value;
    }
}

public class TypedReadOnlyVariable<T> : Variable
{
    public override string Name { get; set; }
    public T Value { get; set; }

    public TypedReadOnlyVariable(string name, T value = default(T))
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

