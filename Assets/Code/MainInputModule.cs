using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainInputModule : StandaloneInputModule
{
    PointerEventData pointer_event_data;

    bool is_below_threshold_drag_occurring = false;
    Vector2 mouse_down_position;

    public bool IsDragOccurring { get; private set; }
    public bool DidDragOccur { get; private set; }

    protected override void ProcessMove(PointerEventData pointerEvent)
    {
        base.ProcessMove(pointerEvent);

        pointer_event_data = pointerEvent;
    }

    private void Update()
    {
        if (InputUtility.WasMouseLeftPressed())
        {
            mouse_down_position = input.mousePosition;
            is_below_threshold_drag_occurring = true;
        }

        if (pointer_event_data != null && 
            pointer_event_data.pointerDrag != null &&
            is_below_threshold_drag_occurring &&
            Vector2.Distance(mouse_down_position, input.mousePosition) >= eventSystem.pixelDragThreshold)
        {
            IsDragOccurring = true;
            is_below_threshold_drag_occurring = false;
        }

        if (InputUtility.WasMouseLeftReleased() || InputUtility.WasMouseRightReleased())
        {
            DidDragOccur = IsDragOccurring;
            IsDragOccurring = false;
            is_below_threshold_drag_occurring = false;
        }
        else
            DidDragOccur = false;
    }

    //using unity terminology, "hovered" here means
    //IsPointedAt && the object blocks raycasts
    public bool IsHovered(GameObject game_object)
    {
        if (pointer_event_data == null)
            return false;

        foreach (GameObject other in pointer_event_data.hovered)
            if (other.transform.IsChildOf(game_object.transform))
                return true;

        return false;
    }

    public bool IsTouched(GameObject game_object)
    {
        if (pointer_event_data == null || 
            pointer_event_data.pointerCurrentRaycast.gameObject == null)
            return false;

        return pointer_event_data.pointerCurrentRaycast.gameObject.transform.IsChildOf(game_object.transform);
    }
}
