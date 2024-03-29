﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : DraggableUIElement
{
    [SerializeField]
    Image background = null;
    public Image Background { get { return background; } }

    [SerializeField]
    Image image = null;
    public Image Image { get { return image; } }

    [SerializeField]
    Image selection_overlay = null;
    public Image SelectionOverlay { get { return selection_overlay; } }

    public bool IsPositioned { get; set; } = false;

    public Drawer Drawer { get { return GetComponentInParent<Drawer>(); } }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);

        transform.position = Input.mousePosition;
        IsPositioned = false;
    }
}
