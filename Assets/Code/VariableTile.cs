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

        object value = Variable.Read();
        string value_string = value.ToString();
        if (value is float)
            value_string = ((float)value).ToString("F1");

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

    public static VariableTile Find(string name)
    {
        System.Predicate<Tile> match = tile => (tile as VariableTile).Variable.Name == name;

        VariableTile variable_tile = Scene.Main.UnitInterface.VariableDrawer.Tiles.Find(match) as VariableTile;
        if (variable_tile != null)
            return variable_tile;

        return Scene.Main.WorldInterface.VariableDrawer.Tiles.Find(match) as VariableTile;
    }
}
