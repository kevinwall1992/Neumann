using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PipeFunctionSlot : MonoBehaviour
{
    PipeFunctionTile pipe_function_tile;

    [SerializeField]
    Image image = null;

    [SerializeField]
    Image selection_overlay = null;

    [SerializeField]
    CanvasGroup canvas_group = null;

    [SerializeField]
    OperationTileIONode primary_input_node = null;
    public OperationTileIONode PrimaryInputNode
    { get { return primary_input_node; } }

    [SerializeField]
    OperationTileIONode secondary_input_node = null;
    public OperationTileIONode SecondaryInputNode
    { get { return secondary_input_node; } }

    public PipeFunctionTile PipeFunctionTile
    {
        get { return pipe_function_tile; }

        set
        {
            if (value != null && pipe_function_tile != null && pipe_function_tile != value)
                GameObject.Destroy(pipe_function_tile.gameObject);

            pipe_function_tile = value;

            if (PipeFunctionTile != null)
            {
                PipeFunctionTile.transform.SetParent(transform);
                primary_input_node.VariablePipe.Function = PipeFunctionTile.Function;
            }
            else
                primary_input_node.VariablePipe.Function = null;
        }
    }

    private void Update()
    {
        bool pipe_function_tile_is_being_dragged =
            DraggableUIElement.DraggedElement != null &&
            DraggableUIElement.DraggedElement.HasComponent<PipeFunctionTile>();

        selection_overlay.gameObject.SetActive(pipe_function_tile_is_being_dragged && 
                                               (transform as RectTransform).ContainsMouse());

        image.color = image.color
            .AlphaChangedTo(PipeFunctionTile == null ? 1 : 0);

        if (PipeFunctionTile != null && !PipeFunctionTile.IsBeingDragged)
            PipeFunctionTile.transform.position = PipeFunctionTile.transform.position
                .Lerped(transform.position, Time.deltaTime * 10);

        if (primary_input_node != null && primary_input_node.IsOpen)
        {
            if (pipe_function_tile_is_being_dragged || PipeFunctionTile != null)
                canvas_group.alpha = primary_input_node.LineAlpha;
            else
                canvas_group.alpha = 0;

            Rect io_node_rect = (primary_input_node.transform as RectTransform).rect;
            (secondary_input_node.transform as RectTransform).sizeDelta =
                new Vector2(io_node_rect.width, io_node_rect.height);

            if (SecondaryInputNode != null)
                primary_input_node.VariablePipe.SecondaryVariableName = 
                    SecondaryInputNode.VariableName;
        }
    }
}
