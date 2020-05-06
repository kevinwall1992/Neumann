using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawerHandle : DraggableUIElement
{
    bool is_hiding_cursor = false;

    public Image Image;
    public Sprite DefaultSprite;
    public Sprite DraggingSprite;
    public Transform Container;
    public Drawer Drawer;
    public float ColorLerpSpeed = 4;

    protected override void Start()
    {
        base.Start();

        Image.color = new Color(1, 1, 1, 0);
    }

    protected override void Update()
    {
        base.Update();

        if (IsBeingDragged)
            Image.sprite = DraggingSprite;
        else
            Image.sprite = DefaultSprite;

        if (this.IsPointedAt() || IsBeingDragged)
        {
            UnityEngine.Cursor.visible = false;
            is_hiding_cursor = true;
        }
        else if (is_hiding_cursor)
            UnityEngine.Cursor.visible = true;

        float target_alpha = 0;
        if (IsBeingDragged || Container.IsPointedAt() || Drawer.IsPointedAt())
            target_alpha = 1;

        Color target_color = new Color(1, 1, 1, target_alpha);
        if (IsBeingDragged || this.IsPointedAt())
            target_color = new Color(1, 0.9f, 0.6f, target_alpha);

        Image.color = Color.Lerp(Image.color, target_color, ColorLerpSpeed * Time.deltaTime);
    }
}
