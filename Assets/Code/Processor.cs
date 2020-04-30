using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(Waster))]
public class Processor : Appliance, HasLoadSite
{
    public float VolumePerSecond;

    public List<ProcessTask> ProcessTasks = new List<ProcessTask>();

    public override IEnumerable<Operation> Abilities { get { return ProcessTasks; } }

    public bool HasLoadSite { get { return Unit.Task is ProcessTask; } }
    public Vector3 LoadSite { get { return (Unit.Task as ProcessTask).Target.Position; } }
    public float LoadSiteRadius { get { return Loader.GetRange(VolumePerSecond); } }

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

public class ProcessBehavior : ApplianceBehavior
{
    float input_concentration;
    float effective_separation_efficiency;

    public override float UsageFraction
    { get { return ProcessTask.GetTransportEfficiency(); } }

    public override bool IsProducingTools
    { get { return ProcessTask.Product.Resources.Contains(Resource.Tools); } }

    public Processor Processor { get { return GetComponent<Processor>(); } }
    public ProcessTask ProcessTask { get { return Unit.Task as ProcessTask; } }

    public override List<Variable> Variables
    {
        get
        {
            return base.Variables.Merged(Utility.List(
                new FunctionVariable(Scene.Main.Style.VariableNames.OptimalConcentration,
                                     () => ProcessTask.OptimalConcentration),
                new FunctionVariable(Scene.Main.Style.VariableNames.InputConcentration,
                                     () => input_concentration),
                new FunctionVariable(Scene.Main.Style.VariableNames.WasteFraction,
                                     () => 1 - effective_separation_efficiency)));
        }
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
        input_concentration = sample.GetVolumeOf(ProcessTask.Feedstock.GetPrincipleComponent()) / 
                              sample.Volume;


        //Separation
        float sample_concentration = 1;
        foreach (Resource resource in ProcessTask.Feedstock.Resources)
        {
            float maximum_concentration = ProcessTask.Feedstock.GetVolumeOf(resource) /
                                          ProcessTask.Feedstock.Volume;

            float actual_concentration = sample.GetVolumeOf(resource) / sample.Volume;

            sample_concentration = Mathf.Min(sample_concentration, actual_concentration /
                                                                   maximum_concentration);
        }

        effective_separation_efficiency = ProcessTask.GetEffectiveSeparationEfficiency(sample_concentration);
        float removed_volume =  effective_separation_efficiency *
                                sample_concentration *
                                sample_volume;

        Pile product =
            ProcessTask.Product.Normalized() *
            sample.TakeOut(ProcessTask.Feedstock.Normalized() * removed_volume).Volume *
            (ProcessTask.Product.Volume / ProcessTask.Feedstock.Volume);

        if (IsProducingTools)
        {
            float tools_produced = product.TakeOut(Resource.Tools).Volume;
            float net_tools = tools_produced -
                              YieldAdjustedUsageFraction * 
                              Appliance.ToolsPerSecond * 
                              Time.deltaTime;

            if (net_tools < 0)
            {
                Unit.Team.Stock.Pile.TakeOut(Resource.Tools, -net_tools);
                Unit.Task = null;
            }
            else
                Unit.Team.Stock.Pile.PutIn(Resource.Tools, net_tools);
        }

        Unit.Team.Stock.Pile.PutIn(product);
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

    public float GetTransportEfficiency(float distance)
    {
        float transport_efficiency = Mathf.Pow(0.5f,
            (distance - OptimalDistance) /
            (HalfDistance - OptimalDistance));

        if (transport_efficiency > 1)
            transport_efficiency = 1;

        return transport_efficiency;
    }

    public float GetTransportEfficiency(Vector3 position)
    {
        return GetTransportEfficiency(Unit.Physical.Position.Distance(position));
    }
}

[System.Serializable]
public class ProcessTask : TransportEfficiencyTask, HasLoadSite
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

    public bool HasLoadSite { get { return true; } }
    public Vector3 LoadSite { get { return SpeculativeTarget.Position; } }
    public float LoadSiteRadius
    {
        get
        {
            Processor processor = Scene.Main.UnitInterface.Unit.GetComponent<Processor>();

            return Loader.GetRange(processor.VolumePerSecond);
        }
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
    {
        get
        {
            Resource principle_component = Product.GetPrincipleComponent();

            return Scene.Main.Style.ProcessTask
                .WithUnderlay(principle_component.Icon, principle_component.Color)
                .WithDescriptionAmended(principle_component.Name);
        }
    }
}
