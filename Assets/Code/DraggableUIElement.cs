using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class DraggableUIElement : UIElement, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    public bool IsBeingDragged
    {
        get { return DraggedElement == this; }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        DraggedElement = this;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        DraggedElement = this;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (IsBeingDragged)
            DraggedElement = null;
    }

    public static UIElement DraggedElement { get; private set; }
}
