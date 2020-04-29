using UnityEngine;
using System.Collections;
using RotaryHeart.Lib.SerializableDictionary;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(Waster))]
public class Refiner : Appliance, HasVariables, HasLoadSite, HasUnloadSite
{
    public float VolumePerSecond;

    public WritableVariable CycleCount { get; private set; } = new WritableVariable("Cycles", 1.0f);

    public List<RefineTask> RefineTasks = new List<RefineTask>();

    public override IEnumerable<Operation> Abilities { get { return RefineTasks; } }

    public List<Variable> Variables { get { return Utility.List<Variable>(CycleCount); } }

    public bool HasLoadSite { get { return Unit.Task is RefineTask; } }
    public Vector3 LoadSite { get { return (Unit.Task as RefineTask).Target.Position; } }
    public float LoadSiteRadius { get { return Loader.GetRange(VolumePerSecond); } }

    public bool HasUnloadSite { get { return Unit.Task is RefineTask; } }
    public Vector3 UnloadSite { get { return (Unit.Task as RefineTask).Destination.Position; } }

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

    public Refiner Refiner { get { return GetComponent<Refiner>(); } }
    public RefineTask RefineTask { get { return Unit.Task as RefineTask; } }

    protected override void Update()
    {
        if (RefineTask == null)
            return;


        //Taking sample
        float sample_volume = Time.deltaTime * Refiner.VolumePerSecond * YieldAdjustedUsageFraction;

        Pile sample = Scene.Main.World.Asteroid.Regolith
            .TakeSample(RefineTask.Source.Position,
                        Loader.GetRange(Refiner.VolumePerSecond),
                        sample_volume);

        Pile ore = new Pile();
        Pile gangue = new Pile().PutIn(sample);

        foreach (Resource resource in RefineTask.Affinities.Keys)
            ore.PutIn(gangue.TakeOut(resource));


        //Computing maximum cycles
        float principle_resource_volume = sample.GetVolumeOf(RefineTask.PrincipleResource);
        float non_principle_volume = sample.Volume - principle_resource_volume;
        float non_principle_affinity = 
            RefineTask.GangueAffinity +
            RefineTask.Affinities.Keys.Sum(resource => 
                (resource == RefineTask.PrincipleResource) ? 
                0 : RefineTask.Affinities[resource]);

        float max_cycle_count = 
            Mathf.Log((1 / RefineTask.MaximumConcentration - 1) * 
                      principle_resource_volume / non_principle_volume) /
            Mathf.Log(non_principle_affinity / 
                      RefineTask.Affinities[RefineTask.PrincipleResource]);

        float cycle_count = Mathf.Min(max_cycle_count, Refiner.CycleCount.Read<float>(1));
        

        //separation of refined sample
        Pile refined_sample = new Pile().PutIn(gangue * Mathf.Pow(RefineTask.GangueAffinity, cycle_count));
        foreach (Resource resource in ore.Resources)
            refined_sample.PutIn(resource, ore.GetVolumeOf(resource) * Mathf.Pow(RefineTask.Affinities[resource], cycle_count));

        Pile waste_sample = new Pile().PutIn(sample);
        waste_sample.TakeOut(refined_sample);

        Scene.Main.World.Asteroid.Litter(RefineTask.Destination.Position, refined_sample);
        Scene.Main.World.Asteroid.Litter(Refiner.Waster.WasteSite, waste_sample);

        base.Update();
    }
}


[System.Serializable]
public class RefineTask : TransportEfficiencyTask, HasLoadSite, HasUnloadSite
{
    [System.Serializable]
    public class ResourceAffinityMap : SerializableDictionaryBase<Resource, float> { }
    public ResourceAffinityMap Affinities = new ResourceAffinityMap();

    [Range(0.0F, 1.0F)]
    public float GangueAffinity;

    [Range(0.0F, 1.0F)]
    public float MaximumConcentration;

    public Resource PrincipleResource
    {
        get { return Affinities.Keys.Sorted(resource => Affinities[resource]).Last(); }
    }

    public Target Source { get { return Target; } }
    public Target Destination { get { return Target.Convert(Output.Read(Unit)); } }

    public override bool IsComplete { get { return false; } }

    public override bool HasOutput { get { return true; } }

    public bool HasLoadSite
    {
        get
        {
            if (Target != null)
                return true;

            return OperationTileNode.Selected != null &&
                   OperationTileNode.Selected.OperationTile.Operation == this &&
                   (OperationTileNode.Selected is OperationTileIONode) &&
                   (OperationTileNode.Selected as OperationTileIONode).IsInputNode;
        }
    }
    public Vector3 LoadSite { get { return SpeculativeTarget.Position; } }
    public float LoadSiteRadius
    {
        get
        {
            Refiner refiner = Scene.Main.UnitInterface.Unit.GetComponent<Refiner>();

            return Loader.GetRange(refiner.VolumePerSecond);
        }
    }
    public bool HasUnloadSite
    {
        get
        {
            if (Unit != null && Output.IsConnected(Unit))
                return true;

            return OperationTileNode.Selected != null &&
                   OperationTileNode.Selected.OperationTile.Operation == this &&
                   (OperationTileNode.Selected is OperationTileIONode) &&
                   !(OperationTileNode.Selected as OperationTileIONode).IsInputNode;
        }
    }
    public Vector3 UnloadSite
    {
        get
        {
            if (Unit != null && Output.IsConnected(Unit))
                return Destination.Position;

            return Scene.Main.World.Asteroid.GetWorldPositionPointedAt();
        }
    }

    public RefineTask(SerializableDictionaryBase<Resource, float> affinities,
                      float gangue_affinity,
                      float maximum_concentration,
                      float optimal_distance,
                      float half_distance)
        : base(optimal_distance, half_distance)
    {
        foreach (Resource resource in affinities.Keys)
            Affinities[resource] = affinities[resource];

        GangueAffinity = gangue_affinity;

        MaximumConcentration = maximum_concentration;
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
        return new RefineTask(Affinities, GangueAffinity, MaximumConcentration, OptimalDistance, HalfDistance);
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.RefineTask; } }
}
