using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OperationTileIONode : OperationTileNode
{
    public string VariableName { get; set; } = null;

    public VariableTile VariableTile { get { return VariableTile.Find(VariableName); } }

    public bool IsInputNode { get { return OperationTile.InputNode == this; } }

    protected override void Start()
    {
        base.Start();

        BezierLineController.DrawStraightLine = true;
    }

    protected override void Update()
    {
        base.Update();

        if (IsSelected || OperationTile.IsPointedAt())
        {
            Hide = false;
            ShowLine();

            if (IsSelected)
            {
                if (Scene.Main.World.IsPointedAt())
                    BezierLineController.EndPosition = Scene.Main.World.Asteroid.GetWorldPositionPointedAt();
                else
                    BezierLineController.EndPosition = Scene.Main.Camera.ScreenToWorldPoint(
                        Input.mousePosition.ZChangedTo(UIDepth));
            }
            else if (VariableTile != null)
            {
                object value = VariableTile.Variable.Read();
                bool is_on_screen = false;
                if (value is Vector3)
                    is_on_screen = (Scene.Main.Canvas.transform as RectTransform)
                        .Contains(Scene.Main.Camera.WorldToScreenPoint((Vector3)value));

                if (is_on_screen)
                    BezierLineController.EndPosition = (Vector3)value;
                else
                    BezierLineController.EndPosition = Scene.Main.Camera.ScreenToWorldPoint(
                        VariableTile.transform.position.ZChangedTo(UIDepth));
            }
            else
                HideLine();
        }
        else
            Hide = true;

        if (!InputUtility.DidDragOccur() && 
            IsSelected && 
            !OperationTile.IsPointedAt() && 
            this.UseMouseLeftRelease())
        {
            if (Scene.Main.InputModule.ElementTouched != null)
            {
                VariableTile variable_tile = InputUtility.GetElementTouched<VariableTile>();
                Unit unit = InputUtility.GetElementTouched<Unit>();

                if (variable_tile != null)
                    VariableName = variable_tile.name;
                else if (unit != null)
                    VariableName = unit.Id;
                else if (Scene.Main.World.IsPointedAt())
                    VariableName = Scene.Main.World.MemorizePosition(
                        Scene.Main.World.Asteroid.GetWorldPositionPointedAt());

            }

            IsSelected = false;
        }
    }
}
