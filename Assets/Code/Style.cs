using UnityEngine;
using System.Collections;
using RotaryHeart.Lib.SerializableDictionary;

[ExecuteAlways]
public class Style : MonoBehaviour
{
    public Cursor DefaultCursor;
    public OperationMenu OperationMenu;
    public int BaseCanvasHeight;
    public float BasePadding = 12;
    public int VerticalTileCount = 10;
    public int BaseViewSize;

    public VariableStyleMap Variables;

    public string EnergyUsageVariableName;

    public Operation MoveTask;
    public Operation AttackTask;
    public Operation BuildTask;
    public Operation ProcessTask;
    public Operation DrillTask;
    public Operation LoadTask;
    public Operation UnloadTask;
    public Operation RefineTask;
    public Operation ConvertTask;

    public Operation ChooseOperation;
    public Operation WriteOperation;
    public Operation InterruptOperation;
    public Operation YieldOperation;
    public Operation SelectWasteSiteOperation;
    public Operation TurnOnOperation;
    public Operation TurnOffOperation;
    public Operation IncreasePowerOperation;
    public Operation DecreasePowerOperation;
    public Operation HighFiringAngleOperation;
    public Operation LowFiringAngleOperation;

    float scale;
    public float Scale { get { return scale; } }

    float tile_size;
    public float TileSize { get { return tile_size; } }

    public float Padding
    {
        get
        {
            return BasePadding * Scale;
        }
    } 

    public float ViewSize
    {
        get { return BaseViewSize * Scale; }
    }

    private void Start()
    {

    }

    private void Update()
    {
        Recompute();
    }

    void Recompute()
    {
        scale = (Scene.Main.Canvas.transform as RectTransform).rect.height / BaseCanvasHeight;

        float vertical_space = (OperationMenu.transform as RectTransform).rect.height - Padding;
        tile_size = vertical_space / VerticalTileCount - Padding;
    }


    [System.Serializable]
    public struct Operation
    {
        public Sprite Sprite;
        public Color Color;
        public string Description;
        public Cursor Cursor;

        [System.NonSerialized]
        public Sprite Underlay;
        [System.NonSerialized]
        public Color UnderlayColor;

        public Operation WithUnderlay(Sprite underlay, Color color)
        {
            Operation copy = this;
            copy.Underlay = underlay;
            copy.UnderlayColor = color;

            return copy;
        }

        public Operation WithDescriptionAmended(string amendment)
        {
            Operation copy = this;
            copy.Description += " " + amendment;

            return copy;
        }
    }

    [System.Serializable]
    public struct Variable
    {
        public Sprite Sprite;
        public Color Color;
    }
    [System.Serializable]
    public class VariableStyleMap : SerializableDictionaryBase<string, Variable>
    {
        new public Variable this[string name]
        {
            get
            {
                if (!ContainsKey(name))
                {
                    Variable variable = new Variable();
                    variable.Color = Color.clear;

                    return variable;
                }

                return base[name];
            }

            set { base[name] = value; }
        }
    }
}
