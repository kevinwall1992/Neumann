using UnityEngine;
using System.Collections;

public class SeekBehavior : Behavior
{
    public Target Target { get; set; }

    public Physical Physical { get { return GetComponent<Physical>(); } }
    public Motile Motile { get { return GetComponent<Motile>(); } }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Motile == null || !Motile.IsFunctioning)
            return;

        Vector3 normal = Scene.Main.World.GetSurfaceNormal(Physical.Position);

        Vector3 displacement = Target.Position - Physical.Position;
        Vector3 surface_direction = displacement.InPlane(Vector3.up).InPlane(normal).normalized;
        float parallel_velocity = Physical.Velocity.Dot(Physical.Direction);

        Motile.Turn(surface_direction);

        if (Physical.Direction.Dot(surface_direction) > 0.9f)
        {
            if ((displacement.magnitude < 10 && parallel_velocity > 6) || (displacement.magnitude < 3 && parallel_velocity > 2))
                Motile.Brake(0.8f);
            else
            {
                Motile.ReleaseBrake();
                Motile.Drive();
            }
        }
        else
            Motile.Brake();
    }

    protected override void OnDestroy()
    {
        Motile.Brake();
        Motile.Drive(0);

        base.OnDestroy();
    }
}
