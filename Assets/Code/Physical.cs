using UnityEngine;
using System.Collections;

[ExecuteAlways]
[RequireComponent(typeof(SphereCollider))]
public class Physical : MonoBehaviour
{
    public float Mass = 1;
    public float Size = 1;
    public Vector3 Velocity;
    public Vector3 Force;

    public Vector3 Position
    {
        get { return transform.position; }
        set { gameObject.transform.position = value; }
    }

    //in degrees
    public float Pitch
    {
        get { return transform.rotation.eulerAngles[0]; }
        set { transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.XChangedTo(value)); }
    }
    public float Yaw
    {
        get { return transform.rotation.eulerAngles[1]; }
        set { transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.YChangedTo(value)); }
    }
    public float Roll
    {
        get { return transform.rotation.eulerAngles[2]; }
        set { transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.ZChangedTo(value)); }
    }

    public Vector3 Direction
    {
        get { return (transform.TransformPoint(new Vector3(0, 0, 1)) - Position).normalized; }
        set
        {
            transform.rotation = Quaternion.Euler(Quaternion.LookRotation(value).eulerAngles.ZChangedTo(Roll));
        }
    }

    public float FrictionCoefficient_Parallel { get; set; }
    public float FrictionCoefficient_Perpendicular { get; set; }

    public float KineticEnergy
    {
        get
        {
            return Mathf.Pow(Measures.WorldUnitsToMeters(Velocity.magnitude), 2) * Mass / 2;
        }

        set
        {
           Velocity = Velocity.normalized * Measures.MetersToWorldUnits(Mathf.Sqrt(2 * value / Mass));
        }
    }

    public SphereCollider Collider { get { return GetComponent<SphereCollider>(); } }

    public bool IsTouchingTerrain
    {
        get
        {
            float terrain_height = Scene.Main.World.Asteroid.GetSurfaceHeight(Position);

            return Position.y < terrain_height + 0.3f;
        }
    }

    void Start()
    {
        Velocity = Vector3.zero;
        Force = Vector3.zero;

        FrictionCoefficient_Parallel = 0.6f;
        FrictionCoefficient_Perpendicular = 0.6f;
    }

    void AccelerationUpdate()
    {
        
    }

    void Update()
    {
        transform.localScale = new Vector3(Size, Size, Size);

        Position += Velocity * Time.deltaTime;
        Velocity += Force / Mass * Time.deltaTime;
        Force = Vector3.zero;
    }
}
