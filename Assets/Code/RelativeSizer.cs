using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class RelativeSizer : MonoBehaviour
{
    public RectTransform Reference;
    public bool ScaleHorizontally = true;
    public float RelativeHorizontalSize;
    public bool ScaleVertically = true;
    public float RelativeVerticalSize;

    public RectTransform RectTransform { get { return transform as RectTransform; } }

    void Start()
    {
        
    }

    void Update()
    {
        if (Reference == null)
            return;

        RectTransform.sizeDelta = 
            new Vector2(ScaleHorizontally ? Reference.rect.size.x * RelativeHorizontalSize : RectTransform.sizeDelta.x, 
                        ScaleVertically ? Reference.rect.size.y * RelativeVerticalSize : RectTransform.sizeDelta.y) ;
    }
}
