using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class BezierLineController : MonoBehaviour
{
    [SerializeField]
    LineRenderer line_renderer = null;

    public Vector3 StartPosition;
    public Vector3 ControlPosition0;
    public Vector3 ControlPosition1;
    public Vector3 EndPosition;

    public PathType Path = PathType.Cubic; 
    public int SampleCount = 10;

    void Start()
    {

    }

    void Update()
    {
        if(Path == PathType.Linear)
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
        switch(Path)
        {
            case PathType.Linear:
                return StartPosition.Lerped(EndPosition, t);

            case PathType.Quadratic:
                return Mathf.Pow(1 - t, 2) * StartPosition +
                       2 * Mathf.Pow(1 - t, 2) * t * ControlPosition0 +
                       t * t * EndPosition;

            case PathType.Cubic:
                return Mathf.Pow(1 - t, 3) * StartPosition +
                       3 * Mathf.Pow(1 - t, 2) * t * ControlPosition0 +
                       3 * (1 - t) * t * t * ControlPosition1 +
                       t * t * t * EndPosition;

            default: return Vector3.zero;
        }
    }

    int TToIndex(float t)
    {
        return (int)(t * (SampleCount - 2));
    }

    public Vector3 GetTangent(int index)
    {
        return line_renderer.GetPosition(index + 1) - line_renderer.GetPosition(index);
    }

    public Vector3 GetTangent(float t)
    {
        return GetTangent(TToIndex(t));
    }

    public Vector3 GetTangentInScreenSpace(int index)
    {
        return Scene.Main.Camera.WorldToScreenPoint(line_renderer.GetPosition(index + 1)) - 
               Scene.Main.Camera.WorldToScreenPoint(line_renderer.GetPosition(index));
    }

    public Vector3 GetTangentInScreenSpace(float t)
    {
        return GetTangentInScreenSpace(TToIndex(t));
    }

    public float GetDistance(Vector3 position)
    {
        if (Path == PathType.Linear)
            return position.Distance(new Line(StartPosition, EndPosition - StartPosition));

        return Enumerable.Range(0, line_renderer.positionCount - 1)
            .Select(index => position.Distance(new Line(line_renderer.GetPosition(index), 
                                                        GetTangent(index))))
            .Min();
    }

    public float GetDistanceInScreenSpace(Vector3 position)
    {
        Camera camera = Scene.Main.Camera;

        if (Path == PathType.Linear)
            return position.Distance(new Line(camera.WorldToScreenPoint(StartPosition), 
                                              camera.WorldToScreenPoint(EndPosition) - 
                                              camera.WorldToScreenPoint(StartPosition)));

        return Enumerable.Range(0, line_renderer.positionCount - 1)
            .Select(index => position.Distance(
                new Line(camera.WorldToScreenPoint(line_renderer.GetPosition(index)),
                         GetTangentInScreenSpace(index))))
            .Min();
    }


    public enum PathType { Linear, Quadratic, Cubic }
}
