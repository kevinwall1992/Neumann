using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class MainInputModule : StandaloneInputModule
{
    PointerEventData pointer_event_data;

    protected override void ProcessMove(PointerEventData pointerEvent)
    {
        base.ProcessMove(pointerEvent);

        pointer_event_data = pointerEvent;
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
