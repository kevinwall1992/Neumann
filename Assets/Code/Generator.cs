using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Converter))]
[RequireComponent(typeof(Toggler))]
public class Generator : Profession
{
    float power_fraction = 1;

    public Pile Feedstock;
    public float EnergyPerSecond;
    public Pile Waste;

    public float PowerFraction
    {
        get { return power_fraction; }

        set
        {
            power_fraction = value;

            if (power_fraction > 1)
                power_fraction = 1;
            else if (power_fraction < 0)
                power_fraction = 0;
        }
    }


    public override IEnumerable<Operation> Abilities
    {
        get
        {
            List<Operation> abilities = 
                Utility.List<Operation>(new ChangePowerOperation(-0.2f), 
                                        new ChangePowerOperation(0.2f), 
                                        new TurnOffOperation(), 
                                        new TurnOnOperation());

            return abilities;                               
        }
    }

    public Converter Converter { get { return GetComponent<Converter>(); } }
    public Toggler Toggler { get { return GetComponent<Toggler>(); } }

    void Start()
    {
        Unit.Memory.Memorize("Power Level", () => PowerFraction);
    }

    void Update()
    {
        if (Unit.Buildable.IsProject)
            return;

        if (Toggler.IsOn)
        {
            Converter.VolumePerSecond = PowerFraction * Feedstock.Volume;

            this.Start<ConvertBehavior>().ConvertTask =
                new ConvertTask(Feedstock,
                                new Pile().PutIn(Resource.Energy,
                                EnergyPerSecond),
                                Waste,
                                0, 0);
        }
        else
            this.Stop<ConvertBehavior>();
    }
}


public class ChangePowerOperation : Operation
{
    public float Delta;

    public ChangePowerOperation(float delta = 0)
    {
        Delta = delta;
    }

    public override void Execute(Unit unit)
    {
        Generator generator = unit.GetComponent<Generator>();
        generator.PowerFraction += Delta;

        base.Execute(unit);
    }

    public override Operation Instantiate()
    {
        return new ChangePowerOperation(Delta);
    }


    public override Style.Operation Style
    {
        get
        {
            if (Delta >= 0)
                return Scene.Main.Style.IncreasePowerOperation;
            else
                return Scene.Main.Style.DecreasePowerOperation;
        }
    }
}
