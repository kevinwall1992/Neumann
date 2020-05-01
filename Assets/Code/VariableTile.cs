using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Text.RegularExpressions;

public class VariableTile : Tile
{
    System.DateTime last_backspace_time;
    float backspace_delay = 0.25f;

    [SerializeField]
    Text input_text = null;

    [SerializeField]
    Image input_highlight = null;

    [SerializeField]
    Image edit_icon = null;

    [SerializeField]
    Text name_text = null;
    public Text NameText { get { return name_text; } }

    [SerializeField]
    Text value_text = null;
    public Text ValueText { get { return value_text; } }

    public Variable Variable { get; set; }

    public bool IsWritable { get { return Variable is WritableVariable; } }

    public bool IsInInputMode { get; set; }

    protected override void Start()
    {
        base.Start();

        edit_icon.gameObject.SetActive(IsWritable);
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
            value_string = ((float)value).ToString(Variable.Style.Format);
        ValueText.text = value_string;

        Image.sprite = Variable.Style.Sprite;
        Image.color = Variable.Style.Color;
        if (Image.sprite == null)
            Image.color = Image.color.AlphaChangedTo(0);


        if (IsWritable &&
            this.IsTouched() &&
            !InputUtility.DidDragOccur &&
            this.UseMouseLeftRelease())
        {
            IsInInputMode = true;

            last_backspace_time = new System.DateTime(0);
        }


        //Text input
        if (IsInInputMode && !this.ClaimKeyboardInput())
            IsInInputMode = false;

        if (IsInInputMode)
        {
            ValueText.gameObject.SetActive(false);

            float target_alpha = 0.25f + Utility.GetLoopedCycleMoment(1.5f) * 0.25f;
            input_highlight.color = input_highlight.color
                .Lerped(input_highlight.color.AlphaChangedTo(target_alpha), 
                        Time.deltaTime * 20);

            input_text.text += new Regex("[^a-zA-Z0-1]").Replace(Input.inputString, "");

            if (Input.GetKey(KeyCode.Backspace))
            {
                if ((System.DateTime.Now - last_backspace_time).TotalSeconds > backspace_delay)
                {
                    input_text.text = input_text.text.Trim(1);
                    last_backspace_time = System.DateTime.Now;
                }
            }

            if (Input.GetKeyUp(KeyCode.Backspace))
                last_backspace_time = new System.DateTime(0);

            if(Input.GetKeyUp(KeyCode.Escape))
            {
                IsInInputMode = false;

                input_text.text = "";
            }

            if (Input.GetKeyUp(KeyCode.Return))
            {
                IsInInputMode = false;

                object write_value = input_text.text;

                float float_value;
                if (float.TryParse(input_text.text, out float_value))
                    write_value = float_value;
                else if (input_text.text.ToLower().Equals("true"))
                    write_value = true;
                else if (input_text.text.ToLower().Equals("false"))
                    write_value = false;

                Variable.Write(write_value);
                input_text.text = "";
            }
        }
        else
        {
            this.YieldKeyboardInputClaim();
            input_highlight.color = input_highlight.color.AlphaChangedTo(0.0f);
            ValueText.gameObject.SetActive(true);
        }
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
