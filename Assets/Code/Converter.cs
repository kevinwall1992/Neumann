using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(Waster))]
public class Converter : Appliance
{
    public float VolumePerSecond;

    public List<ConvertTask> ConvertTasks = new List<ConvertTask>();

    public override IEnumerable<Operation> Abilities { get { return ConvertTasks; } }

    public Waster Waster { get { return GetComponent<Waster>(); } }

    void Start()
    {

    }

    void Update()
    {
        Waster.WasteNot = true;
        foreach (ConvertTask convert_task in ConvertTasks)
            if (convert_task.Waste.Volume > 0)
                Waster.WasteNot = false;

        if (Unit.Task is ConvertTask)
            this.Start<ConvertBehavior>().ConvertTask = Unit.Task as ConvertTask;
        else
            this.Stop<ConvertBehavior>();
    }

    Pile GetBaseUsagePerSecond()
    {
        return ((Unit.Task as ConvertTask).Feedstock.Normalized() * VolumePerSecond)
            .PutIn(Resource.Energy, EnergyPerSecond);
    }
}


public class ConvertBehavior : ApplianceBehavior
{
    public ConvertTask ConvertTask { get; set; }

    public override float UsageFraction
    {
        get
        {
            if (Converter.Waster.WasteNot)
                return 1;

            float distance = Unit.Physical.Position
                .Distance(Converter.Waster.WasteSite);

            return ConvertTask.GetTransportEfficiency(distance);
        }
    }

    public override Pile BaseUsagePerSecond
    {
        get
        {
            return base.BaseUsagePerSecond.Copy()
                .PutIn(ConvertTask.Feedstock.Normalized() * Converter.VolumePerSecond);
        }
    }

    public override bool IsProducingTools
    { get { return ConvertTask.Product.Resources.Contains(Resource.Tools); } }

    Converter Converter { get { return GetComponent<Converter>(); } }

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

        Pile product = ConvertTask.Product.Normalized() *
                       volume *
                       (1 - waste_ratio);

        if (IsProducingTools)
        {
            float tools_produced = product.TakeOut(Resource.Tools).Volume;
            float net_tools = tools_produced -
                              Appliance.ToolsPerSecond * Time.deltaTime;

            if (net_tools < 0)
            {
                Unit.Team.Stock.Pile.TakeOut(Resource.Tools, -net_tools);
                Unit.Task = null;
            }
            else
                Unit.Team.Stock.Pile.PutIn(Resource.Tools, net_tools);
        }

        Unit.Team.Stock.Pile.PutIn(product);

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

    public override Operation Instantiate()
    {
        return new ConvertTask(Feedstock, Product, Waste, OptimalDistance, HalfDistance);
    }


    public override Style.Operation Style
    {
        get
        {
            Resource principle_component = Product.GetPrincipleComponent();

            return Scene.Main.Style.ConvertTask
                .WithUnderlay(principle_component.Icon, principle_component.Color)
                .WithDescriptionAmended(principle_component.Name);
        }
    }
}
