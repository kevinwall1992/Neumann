using UnityEngine;
using System.Collections;

[ExecuteAlways]
public class CircleLineController : MonoBehaviour
{
    [SerializeField]
    LineRenderer line = null;

    public int SampleCount = 20;

    void Start()
    {
        line.loop = true;
    }

    void Update()
    {
        line.positionCount = SampleCount;
        for (int i = 0; i < SampleCount; i++)
        {
            float radians = 2 * Mathf.PI * i / SampleCount;

            line.SetPosition(i, new Vector3(Mathf.Sin(radians), 0, Mathf.Cos(radians)));
        }
    }
}
