using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Toggler : MonoBehaviour, Able
{
    public bool IsOn { get; set; }

    public IEnumerable<Operation> Abilities
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

    void Start()
    {
        IsOn = true;

        if(this.HasComponent<Unit>())
            GetComponent<Unit>().Memory.Memorize("Is On", () => IsOn);
    }

    void Update()
    {

    }
}


public class TurnOnOperation : Operation
{
    public override void Execute(Unit unit)
    {
        unit.GetComponent<Toggler>().IsOn = true;

        base.Execute(unit);
    }

    public override Operation Instantiate()
    {
        return new TurnOnOperation();
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.TurnOnOperation; } }
}

public class TurnOffOperation : Operation
{
    public override void Execute(Unit unit)
    {
        unit.GetComponent<Toggler>().IsOn = false;

        base.Execute(unit);
    }

    public override Operation Instantiate()
    {
        return new TurnOffOperation();
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.TurnOffOperation; } }
}
