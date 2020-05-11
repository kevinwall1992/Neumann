using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OperationTileIONode : OperationTileNode
{
    protected override bool IsLineValid
    { get { return IsSelected || VariableTile != null; } }

    [SerializeField]
    PipeFunctionSlot pipe_function_slot = null;
    public PipeFunctionSlot PipeFunctionSlot { get { return pipe_function_slot; } }

    [SerializeField]
    OperationTileIONode primary_input_node = null;

    public string VariableName { get; set; } = null;

    public VariableTile VariableTile { get { return VariableTile.Find(VariableName); } }

    public bool IsInputNode { get { return OperationTile.InputNode == this; } }

    public VariablePipe VariablePipe { get { return IsInputNode ? 
                                                    OperationTile.Operation.Input : 
                                                    OperationTile.Operation.Output; } }

    public override OperationTile OperationTile
    {
        get
        {
            return IsSecondaryInputNode ? 
                   primary_input_node.OperationTile : 
                   base.OperationTile;
        }
    }

    public override bool IsOpen
    {
        get
        {
            if (IsSecondaryInputNode)
                return primary_input_node.IsOpen;

            return IsSelected || base.IsOpen || OperationTile.IsPointedAt();
        }
    }

    public override float LineAlpha
    {
        get
        {
            return IsSecondaryInputNode ? 
                   primary_input_node.LineAlpha : 
                   base.LineAlpha;
        }
    }

    public bool IsSecondaryInputNode
    { get { return primary_input_node != null || PipeFunctionSlot == null; } }

    protected override void Update()
    {
        if (IsSecondaryInputNode && primary_input_node == null)
            return;

        base.Update();


        bool curved = false;

        if (IsOpen)
        {
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
                {
                    BezierLineController.EndPosition = Scene.Main.Camera.ScreenToWorldPoint(
                        VariableTile.transform.position.ZChangedTo(UIDepth));

                    curved = true;
                }
            }

            if (PipeFunctionSlot != null)
            {
                if(curved)
                    PipeFunctionSlot.transform.position = Scene.Main.Camera.WorldToScreenPoint(
                        BezierLineController.GetPositionAlongPath(0.5f));
                else
                    Scene.Main.Camera.WorldToScreenPoint(BezierLineController.StartPosition).Lerped(
                        Scene.Main.Camera.WorldToScreenPoint(BezierLineController.EndPosition),
                        0.5f);
            }
        }

        Vector3 start_position_in_screen_space = 
            Scene.Main.Camera.WorldToScreenPoint(BezierLineController.StartPosition);
        Vector3 end_position_in_screen_space = 
            Scene.Main.Camera.WorldToScreenPoint(BezierLineController.EndPosition);
        if (curved)
            BezierLineController.ControlPosition0 = Scene.Main.Camera.ScreenToWorldPoint(
                new Vector3(end_position_in_screen_space.x,
                            start_position_in_screen_space.y,
                            (start_position_in_screen_space.z + end_position_in_screen_space.z) / 2));
        else
            BezierLineController.ControlPosition0 = 
                BezierLineController.StartPosition.Lerped(BezierLineController.EndPosition, 0.5f);


        if (!InputUtility.DidDragOccur && 
            IsSelected && 
            !OperationTile.IsPointedAt() && 
            !this.IsPointedAt() &&
            this.UseMouseLeftRelease())
        {
            if (Scene.Main.InputModule.ElementTouched != null)
            {
                VariableTile variable_tile = InputUtility.GetElementTouched<VariableTile>();
                Unit unit = InputUtility.GetElementTouched<Unit>();

                if (variable_tile != null)
                    VariableName = variable_tile.Variable.Name;
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
