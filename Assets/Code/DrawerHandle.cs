using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DrawerHandle : DraggableUIElement, IPointerExitHandler
{
    public Image Image;
    public Sprite DefaultSprite;
    public Sprite DraggingSprite;
    public Transform Container;
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

        if (Container.IsPointedAt())
            UnityEngine.Cursor.visible = !this.IsPointedAt();

        float target_alpha = Container.IsPointedAt() ? 1 : 0;
        Color target_color = this.IsPointedAt() ? new Color(1, 0.9f, 0.6f, target_alpha) :
                                                  new Color(1, 1, 1, target_alpha);

        Image.color = Color.Lerp(Image.color, target_color, ColorLerpSpeed * Time.deltaTime);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UnityEngine.Cursor.visible = true;
    }
}
