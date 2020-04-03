using UnityEngine;
using System.Collections;
using RotaryHeart.Lib.SerializableDictionary;

public class Style : MonoBehaviour
{
    public Cursor DefaultCursor;
    public OperationMenu OperationMenu;
    public int BaseCanvasHeight;
    public float BasePadding = 12;
    public int VerticalTileCount = 10;
    public int BaseViewSize;

    public VariableStyleMap Variables;

    public Operation MoveTask;
    public Operation AttackTask;
    public Operation BuildTask;
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

    public float Scale
    {
        get
        {
            return (Scene.Main.Canvas.transform as RectTransform).rect.height / BaseCanvasHeight;
        }
    }

    public float Padding
    {
        get
        {
            return BasePadding * Scale;
        }
    }

    public float TileSize
    {
        get
        {
            float vertical_space = (OperationMenu.transform as RectTransform).rect.height - Padding;

            return vertical_space / VerticalTileCount - Padding;
        }
    }

    public float ViewSize
    {
        get { return BaseViewSize * Scale; }
    }



    [System.Serializable]
    public struct Operation
    {
        public Sprite Sprite;
        public Color Color;
        public string Description;
        public Cursor Cursor;
    }

    [System.Serializable]
    public struct Variable
    {
        public Sprite Sprite;
        public Color Color;
        public string Units;
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
