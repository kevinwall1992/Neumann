using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class Driller : Profession
{
    public float EnergyPerSecond;
    public float VolumePerSecond;

    public override IEnumerable<Operation> Abilities
    {
        get { return Utility.List(new DrillTask()); }
    }

    void Start()
    {

    }

    void Update()
    {
        if (Unit.Task is DrillTask)
            this.Start<DrillBehavior>();
        else
            this.Stop<DrillBehavior>();
    }
}

public class DrillBehavior : EnergyRequestBehavior
{
    public override float EnergyPerSecond { get { return Driller.EnergyPerSecond; } }
    public override float UsageFraction { get { return DrillTask.GetTransportEfficiency(); } }

    Driller Driller { get { return GetComponent<Driller>(); } }
    DrillTask DrillTask { get { return Unit.Task as DrillTask; } }

    protected override void Update()
    {
        if (DrillTask == null)
            return;

        DrillTask.Drill(Time.deltaTime * Driller.VolumePerSecond * YieldAdjustedUsageFraction);
                             
        base.Update();
    }
}


public class DrillTask : TransportEfficiencyTask
{
    public override bool IsComplete { get { return false; } }

    public override bool TakesInput { get { return false; } }
    public override bool HasOutput { get { return true; } }

    public Target Destination { get { return Target.Convert(Output.Read(Unit)); } }

    public DrillTask()
        : base()
    {

    }

    public void Drill(float volume)
    {
        Pile sample = Scene.Main.World.Asteroid.Rock
            .TakeSample(Unit.Physical.Position,
                        Loader.GetRange(volume),
                        volume);

        Scene.Main.World.Asteroid
            .Litter(Destination.Position, sample);
    }

    public float GetTransportEfficiency()
    {
        return GetTransportEfficiency(Destination.Position);
    }

    public override Operation Instantiate()
    {
        return new DrillTask();
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.DrillTask; } }
}
