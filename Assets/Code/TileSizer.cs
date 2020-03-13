using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class TileSizer : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        (transform as RectTransform).sizeDelta = new Vector2(Scene.Main.Style.TileSize, 
                                                             Scene.Main.Style.TileSize);
    }
}
