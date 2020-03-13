using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VariableTile : Tile
{
    [SerializeField]
    Text name_text = null;
    public Text NameText { get { return name_text; } }

    [SerializeField]
    Text value_text = null;
    public Text ValueText { get { return value_text; } }

    public Variable Variable { get; set; }

    public bool IsWritable { get { return Variable is WritableVariable; } }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Variable == null)
            return;

        NameText.text = Variable.Name;

        object value = Variable.Get();
        string value_string = value.ToString();
        if (value is float)
            value_string = ((float)value).ToString("F1");
        ValueText.text = value_string + Variable.Units;

        Image.sprite = Variable.Sprite;
        Image.color = Variable.Color;
    }

    void SetVariable(Variable variable)
    {
        Variable = variable;
    }


    public static VariableTile Create(Variable variable)
    {
        VariableTile variable_tile = GameObject.Instantiate(Scene.Main.Prefabs.VariableTile);
        variable_tile.Variable = variable;

        return variable_tile;
    }
}
