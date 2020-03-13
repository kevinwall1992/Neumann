using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class ViewSizer : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        (transform as RectTransform).sizeDelta = new Vector2(Scene.Main.Style.ViewSize,
                                                             Scene.Main.Style.ViewSize);
    }
}
