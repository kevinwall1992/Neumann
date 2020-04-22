using UnityEngine;
using System.Collections;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;


[RequireComponent(typeof(Waster))]
public class Refiner : Appliance
{
    public float VolumePerSecond;

    public List<RefineTask> RefineTasks = new List<RefineTask>();

    public override IEnumerable<Operation> Abilities { get { return RefineTasks; } }

    public Waster Waster { get { return GetComponent<Waster>(); } }

    void Start()
    {
        
    }

    void Update()
    {
        if (Unit.Task is RefineTask)
            this.Start<RefineBehavior>();
        else
            this.Stop<RefineBehavior>();
    }
}


public class RefineBehavior : ApplianceBehavior
{
    public override float UsageFraction { get { return RefineTask.GetTransportEfficiency(); } }

    Refiner Refiner { get { return GetComponent<Refiner>(); } }
    RefineTask RefineTask { get { return Unit.Task as RefineTask; } }

    protected override void Update()
    {
        if (RefineTask == null)
            return;

        float sample_volume = Time.deltaTime * Refiner.VolumePerSecond * YieldAdjustedUsageFraction;

        Pile sample = Scene.Main.World.Asteroid.Regolith
            .TakeSample(RefineTask.Source.Position,
                        Loader.GetRange(Refiner.VolumePerSecond),
                        sample_volume);

        Pile ore = new Pile();
        foreach (Resource resource in RefineTask.Affinities.Keys)
        {
            float removed_volume = sample.GetVolumeOf(resource) * RefineTask.Affinities[resource];

            ore.PutIn(sample.TakeOut(resource, removed_volume));
        }
        ore.PutIn(sample.TakeSlice(1 - RefineTask.SeparationEfficiency));

        Scene.Main.World.Asteroid.Litter(RefineTask.Destination.Position, ore);
        Scene.Main.World.Asteroid.Litter(Refiner.Waster.WasteSite, sample);

        base.Update();
    }
}


[System.Serializable]
public class RefineTask : TransportEfficiencyTask
{
    [System.Serializable]
    public class ResourceAffinityMap : SerializableDictionaryBase<Resource, float> { }
    public ResourceAffinityMap Affinities = new ResourceAffinityMap();

    [Range(0.0F, 1.0F)]
    public float SeparationEfficiency;

    public Target Source { get { return Target; } }
    public Target Destination { get { return Target.Convert(Output.Read(Unit)); } }

    public override bool IsComplete { get { return false; } }

    public override bool HasOutput { get { return true; } }

    public RefineTask(SerializableDictionaryBase<Resource, float> affinities,
                      float separation_efficiency,
                      float optimal_distance,
                      float half_distance)
        : base(optimal_distance, half_distance)
    {
        foreach (Resource resource in affinities.Keys)
            Affinities[resource] = affinities[resource];

        SeparationEfficiency = separation_efficiency;
    }

    public RefineTask() { }

    public float GetTransportEfficiency()
    {
        return GetTransportEfficiency(Source.Position) * 
               GetTransportEfficiency(Destination.Position) *
               GetTransportEfficiency(Unit.GetComponent<Waster>().WasteSite);
    }

    public override Operation Instantiate()
    {
        return new RefineTask(Affinities, SeparationEfficiency, OptimalDistance, HalfDistance);
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.RefineTask; } }
}
