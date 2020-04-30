using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attribute : HasVariables
{
    [SerializeField]
    protected float value;

    public string Name;

    public virtual float Value
    {
        get { return value; }
        set { this.value = value; }
    }

    public virtual List<Variable> Variables
    {
        get
        {
            return Utility.List<Variable>(new FunctionVariable(Name, () => Value));
        }
    }

    public Attribute(string name, float value)
    {
        Name = name;
        Value = value;
    }
}

[System.Serializable]
public class FillableAttribute : Attribute
{
    [SerializeField]
    float max_value;

    public override float Value
    {
        get
        {
            Value = value;

            return value;
        }
        set
        {
            this.value = Mathf.Min(MaxValue, Mathf.Max(0, value));
        }
    }

    public virtual float MaxValue
    {
        get
        {
            MaxValue = max_value;

            return max_value;
        }

        set
        {
            max_value = Mathf.Max(0, max_value);
        }
    }

    public override List<Variable> Variables
    {
        get
        {
            Style.Variable variable_style_max = Scene.Main.Style.Variables[Name];
            variable_style_max.Color = Color.Lerp(variable_style_max.Color, Color.white, 0.5f);

            return base.Variables.Merged(Utility.List<Variable>(
                new FunctionVariable("Max " + Name, () => MaxValue)));
        }
    }

    public FillableAttribute(string name, float value, float max_value)
        : base(name, value)
    { 
        MaxValue = max_value;
    }

    public FillableAttribute(string name, float max_value)
        : base(name, max_value)
    {
        MaxValue = max_value;
    }


}