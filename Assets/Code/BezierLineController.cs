using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BezierLineController : MonoBehaviour
{
    [SerializeField]
    LineRenderer line_renderer = null;

    public Vector3 StartPosition;
    public Vector3 ControlPosition0;
    public Vector3 ControlPosition1;
    public Vector3 EndPosition;

    public int SampleCount = 10;

    public bool DrawStraightLine { get; set; } = false;

    void Start()
    {

    }

    void Update()
    {
        if(DrawStraightLine)
        {
            line_renderer.positionCount = 2;
            line_renderer.SetPosition(0, StartPosition);
            line_renderer.SetPosition(1, EndPosition);
            return;
        }

        int effective_sample_count = Mathf.Max(1, (SampleCount - 1));

        List<Vector3> positions = new List<Vector3>();
        for (int i = 0; i <= effective_sample_count; i++)
            positions.Add(GetPositionAlongPath(i * 1.0f / effective_sample_count));

        line_renderer.positionCount = positions.Count;
        line_renderer.SetPositions(positions.ToArray());
    }

    public Vector3 GetPositionAlongPath(float t)
    {
        if (DrawStraightLine)
            return StartPosition.Lerped(EndPosition, t);

        return Mathf.Pow(1 - t, 3) * StartPosition +
               3 * Mathf.Pow(1 - t, 2) * t * ControlPosition0 +
               3 * (1 - t) * t * t * ControlPosition1 +
               t * t * t * EndPosition;
    }
}
