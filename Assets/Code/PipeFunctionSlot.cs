using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PipeFunctionSlot : MonoBehaviour
{
    PipeFunctionTile pipe_function_tile;

    [SerializeField]
    OperationTileIONode io_node = null;

    [SerializeField]
    Image image = null;

    [SerializeField]
    Image selection_overlay = null;

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
                io_node.VariablePipe.Function = PipeFunctionTile.Function;
            }
            else
                io_node.VariablePipe.Function = null;
        }
    }

    private void Update()
    {
        bool pipe_function_tile_is_being_dragged = 
            DraggableUIElement.DraggedElement != null &&
            DraggableUIElement.DraggedElement.HasComponent<PipeFunctionTile>();

        float selection_overlay_alpha = 0;
        if (pipe_function_tile_is_being_dragged && (transform as RectTransform).ContainsMouse())
            selection_overlay_alpha = io_node.LineAlpha * 0.75f;
        selection_overlay.color = selection_overlay.color.AlphaChangedTo(selection_overlay_alpha);


        image.color = image.color.AlphaChangedTo(0);
        
        if (PipeFunctionTile != null)
        {
            if(!PipeFunctionTile.IsBeingDragged)
                PipeFunctionTile.transform.position =
                    PipeFunctionTile.transform.position.Lerped(transform.position,
                                                               Time.deltaTime * 10);

            PipeFunctionTile.Background.color = 
                PipeFunctionTile.Background.color.AlphaChangedTo(io_node.LineAlpha);
        }
        else if(pipe_function_tile_is_being_dragged)
            image.color = image.color.AlphaChangedTo(io_node.LineAlpha);

    }
}
