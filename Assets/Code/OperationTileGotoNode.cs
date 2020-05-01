using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class OperationTileGotoNode : OperationTileNode
{
    float stretch_distance = 80;

    [SerializeField]
    GameObject arrow = null;

    public OperationTile GotoOperationTile { get; set; }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        BezierLineController.ControlPosition0 = Scene.Main.Camera.ScreenToWorldPoint(
            new Vector3(transform.position.x + stretch_distance * Scene.Main.Style.Scale, 
                        transform.position.y, 
                        UIDepth));

        arrow.SetActive(false);
        arrow.transform.rotation = 
            Quaternion.LookRotation(arrow.transform.position - Scene.Main.Camera.transform.position, 
                                    Scene.Main.Camera.transform.up);

        if (GotoOperationTile != null || IsSelected || OperationTile.IsPointedAt())
        {
            Hide = false;
            arrow.SetActive(true);

            Vector3 screen_space_end_position = Vector3.zero;

            if (IsSelected)
            {
                ShowLine();

                screen_space_end_position = Input.mousePosition.ZChangedTo(UIDepth);
            }
            else if (GotoOperationTile != null)
            {
                ShowLine();

                screen_space_end_position = 
                    GotoOperationTile.GotoAttachPosition.transform.position.ZChangedTo(UIDepth);
            }
            else
            {
                HideLine();
                arrow.SetActive(false);
            }

            BezierLineController.EndPosition = Scene.Main.Camera.ScreenToWorldPoint(screen_space_end_position);
            arrow.transform.position = BezierLineController.EndPosition;

            BezierLineController.ControlPosition1 = Scene.Main.Camera.ScreenToWorldPoint(
                new Vector3(screen_space_end_position.x + stretch_distance * Scene.Main.Style.Scale,
                            screen_space_end_position.y,
                            UIDepth));
        }
        else
            Hide = true;

        if (!InputUtility.DidDragOccur && IsSelected && !OperationTile.IsPointedAt() && this.UseMouseLeftRelease())
        {
            if (Scene.Main.InputModule.ElementTouched != null)
            {
                OperationTile operation_tile = InputUtility.GetElementTouched<OperationTile>();

                if (operation_tile != null && !operation_tile.IsInOperationMenu)
                    GotoOperationTile = operation_tile;
                else
                    GotoOperationTile = null;
            }
            else
                GotoOperationTile = null;

            IsSelected = false;
        }
    }
}
