using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Billboard))]
public class LineArrowController : MonoBehaviour
{
    [SerializeField]
    LineRenderer line = null;

    //This hides obsolete field Component.renderer.
    //I Ddon't think this can cause a problem, 
    //but I'm leaving this note just in case.
    [SerializeField]
    new Renderer renderer = null;

    Vector3 LineFinalPosition
    { get { return line.GetPosition(line.positionCount - 1); } }

    Vector3 LinePenultimatePosition
    { get { return line.GetPosition(line.positionCount - 2); } }

    void Start()
    {

    }

    void Update()
    {
        transform.position = LineFinalPosition;

        renderer.enabled = line.enabled;
    }

    private void LateUpdate()
    {
        Camera camera = Scene.Main.Camera;
        Vector3 line_final_screen_position = camera.WorldToScreenPoint(LineFinalPosition);
        Vector3 line_penultimate_screen_position = camera.WorldToScreenPoint(LinePenultimatePosition);
        Vector3 direction = line_final_screen_position - line_penultimate_screen_position;
        direction.z = 0;

        float angle = direction.AngleBetween(new Vector3(1, 0, 0));
        if (direction.y < 0)
            angle *= -1;

        transform.rotation = transform.rotation * Quaternion.AngleAxis(MathUtility.RadiansToDegrees(angle), new Vector3(0, 0, 1));
    }
}
