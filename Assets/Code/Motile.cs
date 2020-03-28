using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;


[RequireComponent(typeof(Physical))]
[RequireComponent(typeof(Buildable))]
public class Motile : Able
{
    Vector3 target_direction = new Vector3(-1, 0, 0);

    Stock.Request request = null;

    public float Force = 4;
    public Pile Propellent = new Pile();
    public float TopSpeed = 5;
    public float FrictionCoefficient_Unbraked = 0.1f;
    public float FrictionCoefficient_Braked = 1.0f;
    public float FrictionCoefficient_Skid = 1.0f;
    public float TurningSpeed = 1;

    //[0-1]
    public float DrivePercent { get; set; }
    public float BrakePercent { get; set; }

    public override IEnumerable<Task> Abilities
    {
        get { return Utility.List<Task>(new MoveTask()); }
    }

    public bool IsFunctioning { get { return !Buildable.IsProject; } }

    public Physical Physical { get { return GetComponent<Physical>(); } }
    public Buildable Buildable { get { return GetComponent<Buildable>(); } }


    void Update()
    {
        Physical.FrictionCoefficient_Perpendicular = FrictionCoefficient_Skid;
        Physical.FrictionCoefficient_Parallel = Mathf.Lerp(FrictionCoefficient_Unbraked, FrictionCoefficient_Braked, BrakePercent);
        

        if (Task is MoveTask)
            this.Start<SeekBehavior>().Target = (Task as MoveTask).Target;
        else
            this.Stop<SeekBehavior>();

        if (!Physical.IsTouchingTerrain)
            return;

        if(request == null)
            request = Unit.Team.Stock.MakeRequest(Propellent * 0);

        //Turn towards direction indicated
        Quaternion target_rotation = Quaternion.LookRotation(target_direction);
        float target_yaw = target_rotation.eulerAngles.y;
        Physical.Yaw = Mathf.LerpAngle(Physical.Yaw, target_yaw, Time.deltaTime * TurningSpeed);
        float target_pitch = target_rotation.eulerAngles.x;
        Physical.Pitch = Mathf.LerpAngle(Physical.Pitch, target_pitch, Time.deltaTime * TurningSpeed);

        //Apply force in forward direction
        float speed = Physical.Velocity.Dot(Physical.Direction);
        Physical.Force += Mathf.Min(1, 0.999f - speed / TopSpeed) * 
                          request.Yield * 
                          (request.UsagePerSecond.Volume / Propellent.Volume) * 
                          Force * 
                          Physical.Direction.normalized;

        //Surface gripping
        if(Physical.IsTouchingTerrain)
            Physical.Force += -20 * Scene.Main.World.Asteroid.GetSurfaceNormal(Physical.Position);

        request.UsagePerSecond = Propellent * DrivePercent;
    }

    //[0,1]
    public void Drive(float percent = 1)
    {
        DrivePercent = percent;
    }

    public void Neutral()
    {
        DrivePercent = 0;
    }

    //[0,1]
    public void Brake(float percent = 1)
    {
        BrakePercent = percent;
    }

    public void ReleaseBrake()
    {
        BrakePercent = 0;
    }

    public void Turn(Vector3 direction)
    {
        target_direction = direction;
    }
}


[System.Serializable]
public class MoveTask : Task
{
    public override bool IsComplete
    {
        get { return (Unit.Physical.Position - Target.Position).magnitude < 1; }
    }

    public override Operation Instantiate()
    {
        return new MoveTask();
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.MoveTask; } }
}
