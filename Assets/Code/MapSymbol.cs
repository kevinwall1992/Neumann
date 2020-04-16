using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Billboard))]
public class MapSymbol : MonoBehaviour
{
    public float RelativeScreenSpaceSize;

    void Start()
    {

    }

    void Update()
    {
        float radians = MathUtility.DegreesToRadians(Scene.Main.Camera.fieldOfView / 2);
        float world_height_at_distance =
            Scene.Main.Camera.transform.position.Distance(transform.position) *
            2 * Mathf.Sin(radians) /
            Mathf.Cos(radians);

        transform.localScale = new Vector3(1, 1, 1) *
                               RelativeScreenSpaceSize *
                               world_height_at_distance /
                               transform.parent.lossyScale.y;
    }
}
