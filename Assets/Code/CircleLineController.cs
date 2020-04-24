using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class CircleLineController : MonoBehaviour
{
    [SerializeField]
    LineRenderer line = null;

    public int SampleCount = 36;
    public float Radius;

    public LineRenderer Line { get { return line; } }

    void Start()
    {
        Line.loop = true;
    }

    void Update()
    {
        Line.positionCount = SampleCount;
        for (int i = 0; i < SampleCount; i++)
        {
            float radians = 2 * Mathf.PI * i / SampleCount;

            Line.SetPosition(i, new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)) * Radius) ;
        }
    }
}
