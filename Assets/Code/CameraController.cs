using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public float PanSpeed = 10;
    public float ZoomSpeed = 10;

    public Vector3 Forward
    {
        get { return (transform.TransformPoint(0, 0, 1) - transform.position).normalized; }
    }
    public Vector3 Right
    {
        get { return (transform.TransformPoint(1, 0, 0) - transform.position).normalized; }
    }

    void Start()
    {

    }

    void Update()
    {
        if (this.CanUseKeyboardInput())
        {
            if (Input.GetKey(KeyCode.W))
                transform.position += new Vector3(-1, 0, 0) * PanSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.S))
                transform.position += new Vector3(1, 0, 0) * PanSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.D))
                transform.position += new Vector3(0, 0, 1) * PanSpeed * Time.deltaTime;
            if (Input.GetKey(KeyCode.A))
                transform.position += new Vector3(0, 0, -1) * PanSpeed * Time.deltaTime;
        }

        transform.position += Forward * Input.mouseScrollDelta.y * ZoomSpeed * Time.deltaTime;
    }
}
