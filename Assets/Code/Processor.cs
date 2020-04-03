using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(Waster))]
public class Processor : Able
{
    public float EnergyPerSecond;
    public float VolumePerSecond;

    public List<ProcessTask> ProcessTasks = new List<ProcessTask>();

    public override IEnumerable<Task> Abilities { get { return ProcessTasks; } }

    public Waster Waster { get { return GetComponent<Waster>(); } }

    private void Start()
    {
        
    }

    private void Update()
    {
        if (Unit.Task is ProcessTask)
            this.Start<ProcessBehavior>();
        else
            this.Stop<ProcessBehavior>();
    }
}

public class ProcessBehavior : EnergyRequestBehavior
{
    public override float EnergyPerSecond { get { return Processor.EnergyPerSecond; } }
    public override float UsageFraction { get { return ProcessTask.GetTransportEfficiency(); } }

    Processor Processor { get { return GetComponent<Processor>(); } }
    ProcessTask ProcessTask { get { return Unit.Task as ProcessTask; } }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (ProcessTask == null)
            return;
        
        //Transportation volume and energy usage
        float range = Loader.GetRange(Processor.VolumePerSecond);

        float sample_volume= Processor.VolumePerSecond *
                             Time.deltaTime *
                             YieldAdjustedUsageFraction;
        Pile sample = Scene.Main.World.Asteroid.Regolith
            .TakeSample(ProcessTask.Target.Position, range, sample_volume);


        //Separation
        float sample_concentration = 1;
        foreach (Resource resource in ProcessTask.Feedstock.Resources)
        {
            float maximum_concentration = ProcessTask.Feedstock.GetVolumeOf(resource) /
                                          ProcessTask.Feedstock.Volume;

            float actual_concentration = Scene.Main.World.Asteroid.Regolith
                .GetConcentrationByVolume(ProcessTask.Target.Position, range, resource);

            sample_concentration = Mathf.Min(sample_concentration, actual_concentration /
                                                                   maximum_concentration);
        }

        float removed_volume = ProcessTask.GetEffectiveSeparationEfficiency(sample_concentration) *
                                sample_concentration *
                                sample_volume;

        Unit.Team.Stock.Pile
            .PutIn(ProcessTask.Product.Normalized() * 
                   sample.TakeOut(ProcessTask.Feedstock.Normalized() * removed_volume).Volume * 
                   (ProcessTask.Product.Volume / ProcessTask.Feedstock.Volume));

        Scene.Main.World.Asteroid.Litter(Processor.Waster.WasteSite, sample);

        base.Update();
    }
}


[System.Serializable]
public abstract class TransportEfficiencyTask : Task
{
    public float OptimalDistance = 5;
    public float HalfDistance = 10;

    public TransportEfficiencyTask(float optimal_distance, float half_distance)
    {
        OptimalDistance = optimal_distance;
        HalfDistance = half_distance;
    }

    public TransportEfficiencyTask() { }

    public float GetTransportEfficiency(Vector3 position)
    {
        float transport_efficiency = Mathf.Pow(0.5f,
            (Unit.Physical.Position.Distance(position) - OptimalDistance) /
            (HalfDistance - OptimalDistance));

        if (transport_efficiency > 1)
            transport_efficiency = 1;

        return transport_efficiency;
    }
}

[System.Serializable]
public class ProcessTask : TransportEfficiencyTask
{
    public Pile Feedstock;
    public Pile Product;

    public float OptimalConcentration;
    public float HalfConcentration;

    [Range(0.0F, 1.0F)]
    public float SeparationEfficiency;

    public override bool IsComplete
    {
        get { return false; }
    }

    public ProcessTask(Pile feedstock,
                       Pile product,
                       float optimal_concentration,
                       float half_concentration,
                       float separation_efficiency,
                       float optimal_distance,
                       float half_distance)
        : base(optimal_distance, half_distance)
    {
        Feedstock = feedstock;
        Product = product;
        OptimalConcentration = optimal_concentration;
        HalfConcentration = half_concentration;
        SeparationEfficiency = separation_efficiency;
    }

    public ProcessTask() { }

    public float GetTransportEfficiency()
    {
        return base.GetTransportEfficiency(Target.Position) * 
               base.GetTransportEfficiency(Unit.GetComponent<Waster>().WasteSite);
    }

    public float GetEffectiveSeparationEfficiency(float concentration)
    {
        float concentration_efficiency = Mathf.Pow(0.5f,
            (concentration - OptimalConcentration) /
            (HalfConcentration - OptimalConcentration));

        if (concentration_efficiency > 1)
            concentration_efficiency += (concentration_efficiency - 1) / 4;

        return SeparationEfficiency * concentration_efficiency;
    }

    public override Operation Instantiate()
    {
        return new ProcessTask(Feedstock.Copy(),
                               Product.Copy(),
                               OptimalConcentration,
                               HalfConcentration,
                               SeparationEfficiency,
                               OptimalDistance,
                               HalfDistance);
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.ProcessTask; } }
}
