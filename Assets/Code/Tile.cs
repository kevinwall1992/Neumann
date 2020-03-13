using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : DraggableUIElement
{
    [SerializeField]
    Image image = null;
    public Image Image { get { return image; } }

    [SerializeField]
    Image selection_overlay = null;
    public Image SelectionOverlay { get { return selection_overlay; } }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        transform.position = Input.mousePosition;
    }
}
