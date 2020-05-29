using UnityEngine;
using System.Collections;

public class SeekBehavior : Behavior
{
    Graph.Path path;
    int progress = 0;

    public Target Target { get; set; }

    public Physical Physical { get { return GetComponent<Physical>(); } }
    public Motile Motile { get { return GetComponent<Motile>(); } }

    protected override void Start()
    {
        base.Start();

        path = Scene.Main.World.Asteroid.HighwaySystem
            .PlanRoadtrip(transform.position, Target.Position);
    }

    protected override void Update()
    {
        base.Update();

        if (Motile == null || !Motile.IsFunctioning)
            return;

        Vector3 next_stop;
        if (path != null)
        {
            Graph.Node next_node = path[progress + 1];

            float cost = GraphUtility.ObstacleMetric(GraphUtility.CreatePositionNode(Physical.Position), next_node);

            float remaining_lead = path_lead;
            while(remaining_lead > cost && path.IndexOf(next_node) < (path.Count - 1))
            {
                Graph.Node following_node = path[path.IndexOf(next_node) + 1];

                remaining_lead -= cost;
                cost = GraphUtility.ObstacleMetric(next_node, following_node);
                next_node = following_node;
            }

            next_stop = next_node.GetPosition();
            progress = path.IndexOf(next_node) - 1;
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


    static float path_lead = 15;
}
