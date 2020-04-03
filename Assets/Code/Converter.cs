using UnityEngine;
using System.Collections;
using System.Collections.Generic;


[RequireComponent(typeof(Waster))]
public class Converter : Able
{
    public float EnergyPerSecond;
    public float VolumePerSecond;

    public List<ConvertTask> ConvertTasks = new List<ConvertTask>();

    public override IEnumerable<Task> Abilities { get { return ConvertTasks; } }

    public Waster Waster { get { return GetComponent<Waster>(); } }

    void Start()
    {

    }

    void Update()
    {
        if (Unit.Task is ConvertTask)
            this.Start<ConvertBehavior>();
        else
            this.Stop<ConvertBehavior>();
    }

    Pile GetBaseUsagePerSecond()
    {
        return ((Unit.Task as ConvertTask).Feedstock.Normalized() * VolumePerSecond)
            .PutIn(Resource.Energy, EnergyPerSecond);
    }
}


public class ConvertBehavior : EnergyRequestBehavior
{
    public override float EnergyPerSecond { get { return Converter.EnergyPerSecond; } }
    public override float UsageFraction { get { return ConvertTask.GetTransportEfficiency(); } }

    public override Pile BaseUsagePerSecond
    {
        get
        {
            return base.BaseUsagePerSecond.Copy()
                .PutIn(ConvertTask.Feedstock.Normalized() * Converter.VolumePerSecond);
        }
    }

    Converter Converter { get { return GetComponent<Converter>(); } }
    ConvertTask ConvertTask { get { return Unit.Task as ConvertTask; } }

    protected override void Update()
    {
        if (ConvertTask == null)
            return;

        float volume = Time.deltaTime *
                       Converter.VolumePerSecond *
                       YieldAdjustedUsageFraction *
                       (ConvertTask.Product.Volume + ConvertTask.Waste.Volume) /
                       ConvertTask.Feedstock.Volume;

        float waste_ratio = ConvertTask.Waste.Volume /
                            (ConvertTask.Product.Volume + ConvertTask.Waste.Volume);

        Unit.Team.Stock.Pile.PutIn(ConvertTask.Product.Normalized() *
                                   volume *
                                   (1 - waste_ratio));

        Scene.Main.World.Asteroid
            .Litter(Converter.Waster.WasteSite, ConvertTask.Waste.Normalized() *
                                                volume *
                                                waste_ratio);

        base.Update();
    }
}


[System.Serializable]
public class ConvertTask : TransportEfficiencyTask
{
    public Pile Feedstock = new Pile();
    public Pile Product = new Pile();

    public Pile Waste = new Pile();

    public override bool IsComplete { get { return false; } }

    public override bool TakesInput { get { return false; } }

    public ConvertTask(Pile feedstock, Pile product, Pile waste, 
                       float optimal_distance, float half_distance)
        : base(optimal_distance, half_distance)
    {
        Feedstock = feedstock;
        Product = product;

        Waste = waste;
    }

    public ConvertTask()
    {

    }

    public float GetTransportEfficiency()
    {
        return base.GetTransportEfficiency(Unit.GetComponent<Waster>().WasteSite);
    }

    public override Operation Instantiate()
    {
        return new ConvertTask(Feedstock, Product, Waste, OptimalDistance, HalfDistance);
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.ConvertTask; } }
}
