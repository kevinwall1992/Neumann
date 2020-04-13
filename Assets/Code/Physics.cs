using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Physics : MonoBehaviour
{
    IEnumerable<Physical> Physicals { get { return GetComponentsInChildren<Physical>(); } }

    World World { get { return GetComponentInParent<World>(); } }

    void Start()
    {
        
    }

    void Update()
    {
        Vector3 gravity = new Vector3(0, -Scene.Main.World.Asteroid.SurfaceGravity, 0);

        foreach (Physical physical in Physicals)
        {
            bool is_stationary = IsStationary(physical);

            //Interactions
            foreach (Physical neighbor in Physicals)
            {
                if (physical == neighbor)
                    continue;

                if (is_stationary != IsStationary(neighbor))
                    continue;

                float distance = physical.Position.Distance(neighbor.Position);
                float touch_distance = physical.Size + neighbor.Size;

                if (distance < touch_distance)
                    physical.Force += 100 * neighbor.Mass * (physical.Position - neighbor.Position).normalized * (touch_distance - distance) / touch_distance;
            }


            //General forces

            float terrain_height = World.Asteroid.GetSurfaceHeight(physical.Position);
            bool below_terrain = physical.Position.y < terrain_height;

            if (physical.IsTouchingTerrain)
            {
                Vector3 normal = World.Asteroid.GetSurfaceNormal(physical.Position);
                

                Vector3 normal_force =  physical.Mass * 0.99f * -gravity.InAxis(normal);
                Vector3 normal_velocity = physical.Velocity.InAxis(normal);
                Vector3 sliding_velocity = physical.Velocity - normal_velocity;
                Vector3 parallel_velocity = sliding_velocity.InAxis(physical.Direction);
                Vector3 perpendicular_velocity = sliding_velocity - parallel_velocity;

                //Friction in forward direction
                if (parallel_velocity.magnitude > 0.01f)
                    physical.Force += -parallel_velocity.normalized *
                                      normal_force.magnitude *
                                      physical.FrictionCoefficient_Parallel;

                //Friction in Left/Right direction
                if (perpendicular_velocity.magnitude > 0.01f)
                    physical.Force += -perpendicular_velocity.normalized *
                                      normal_force.magnitude *
                                      physical.FrictionCoefficient_Perpendicular;

                //Normal Force
                if(!is_stationary)
                    physical.Force += normal_force;

                if(below_terrain)
                {
                    physical.Position = physical.Position.YChangedTo(terrain_height);

                    physical.Velocity += -(0.1f * 1 + 1) * normal_velocity;
                }

                Vector3 target_direction = physical.Direction.InPlane(normal);
                physical.Direction = physical.Direction.Lerped(target_direction, 5 * Time.deltaTime);

                float target_roll = Mathf.Asin(normal.Crossed(physical.Direction).y);
                physical.Roll = Mathf.LerpAngle(physical.Roll, MathUtility.RadiansToDegrees(target_roll), 5 * Time.deltaTime);
            }

            if (is_stationary)
            {
                physical.Position = physical.Position.YChangedTo(terrain_height);
                physical.Velocity = physical.Velocity.YChangedTo(0);
            }
            else
            {
                //Gravity
                physical.Force += physical.Mass * gravity;
            }
        }
    }

    bool IsStationary(Physical physical)
    {
        return physical.HasComponent<Motile>();
    }
}
