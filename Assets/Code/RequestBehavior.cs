using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class RequestBehavior : Behavior
{
    Stock.Request request;

    public abstract Pile BaseUsagePerSecond { get; }
    public virtual float UsageFraction { get { return 1; } }

    public float YieldAdjustedUsageFraction
    {
        get { return request.Yield * request.UsagePerSecond.Volume / BaseUsagePerSecond.Volume; }
    }

    public Unit Unit { get { return GetComponent<Unit>(); } }

    protected override void Start()
    {
        base.Start();

        request = Unit.Team.Stock.MakeRequest();
    }

    protected override void Update()
    {
        base.Update();

        request.UsagePerSecond = BaseUsagePerSecond * UsageFraction;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        request.Revoke();
    }
}

public abstract class EnergyRequestBehavior : RequestBehavior, HasVariables
{
    public abstract float EnergyPerSecond { get; }

    public virtual List<Variable> Variables
    {
        get
        {
            return Utility.List<Variable>(
                new FunctionVariable(Scene.Main.Style.VariableNames.EnergyUsage,
                                     () => EnergyPerSecond));
        }
    }

    public override Pile BaseUsagePerSecond
    {
        get { return new Pile().PutIn(Resource.Energy, EnergyPerSecond); }
    }
}