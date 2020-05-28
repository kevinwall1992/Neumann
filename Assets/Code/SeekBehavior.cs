using UnityEngine;
using System.Collections;

public class SeekBehavior : Behavior
{
    Graph.Path path;
    int progress;

    public Target Target { get; set; }

    public Physical Physical { get { return GetComponent<Physical>(); } }
    public Motile Motile { get { return GetComponent<Motile>(); } }

    protected override void Start()
    {
        base.Start();

        path = Scene.Main.World.Asteroid.HighwaySystem.PlanRoadtrip(transform.position, Target.Position);
        progress = 0;
    }

    protected override void Update()
    {
        base.Update();

        if (Motile == null || !Motile.IsFunctioning)
            return;

        Vector3 next_stop;
        if (path != null)
        {
            int progress_estimate = path.IndexOf(
                path.GetNearestNode(Physical.Position, GraphUtility.ObstacleMetric));
            progress = Mathf.Max(progress_estimate, progress);
            int next_stop_index = Mathf.Min(path.Count - 1, progress + 1 + path_smoothing);

            next_stop = (path[next_stop_index].Data as Graph.PositionData).Position;
        }
        else
            next_stop = Target.Position;

        Vector3 normal = Scene.Main.World.Asteroid.GetSurfaceNormal(Physical.Position);

        Vector3 displacement = next_stop - Physical.Position;
        Vector3 surface_direction = displacement.InPlane(Vector3.up).InPlane(normal).normalized;
        float parallel_velocity = Physical.Velocity.Dot(Physical.Direction);

        Motile.Turn(surface_direction);

        if (Physical.Direction.Dot(surface_direction) > 0.9f)
        {
            if ((displacement.magnitude < 10 && parallel_velocity > 6) || 
                (displacement.magnitude < 3 && parallel_velocity > 2))
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


    static int path_smoothing = 0;
}
