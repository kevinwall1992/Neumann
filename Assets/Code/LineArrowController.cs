using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Billboard))]
public class LineArrowController : MonoBehaviour
{
    [SerializeField]
    LineRenderer line = null;

    //This hides obsolete field Component.renderer.
    //I don't think this can cause a problem, 
    //but I'm leaving this note just in case.
    [SerializeField]
    new Renderer renderer = null;

    [SerializeField]
    ColorMaterialProperty color_material_property = null;

    public Color Color;

    public float ArrowSize = 1;

    Vector3 LineFirstPosition
    { get { return line.GetPosition(0); } }

    Vector3 LineFinalPosition
    { get { return line.GetPosition(line.positionCount - 1); } }

    Vector3 LinePenultimatePosition
    { get { return line.GetPosition(line.positionCount - 2); } }

    void Start()
    {

    }

    void Update()
    {
        renderer.enabled = line.enabled;

        color_material_property.Value = Color;

        transform.localScale = VectorUtility.One * ArrowSize;
        transform.position = LineFinalPosition + (LineFirstPosition - LineFinalPosition).normalized * ArrowSize / 2;
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
