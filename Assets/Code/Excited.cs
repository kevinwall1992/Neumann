using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Excited : Status
{
    public override string Name { get { return "Excited"; } }

    public float Excitement { get; set; }

    public override List<Variable> Variables
    {
        get
        {
            List<Variable> variables = new List<Variable>(base.Variables);

            variables.Add(new FunctionVariable("Excitement", 
                () => Excitement / Unit.Mortal.Health.MaxValue));

            return variables;
        }
    }

    private void Update()
    {
        float damage = Excitement * 
                       (Excitement / Unit.Mortal.Health.MaxValue) * 
                       (1 - Unit.Mortal.RadiationHardness) * 
                       Time.deltaTime;

        Unit.Mortal.Health.Value -= damage;

        Excitement -= damage + Time.deltaTime * Unit.Mortal.Health.MaxValue / 100;
        if (Excitement <= 0)
            GameObject.Destroy(this);
    }

    public void Excite(float joules)
    {
        Excitement += ElectromagneticEnergyToDamagePerSecond(joules);
    }

    public static float ElectromagneticEnergyToDamagePerSecond(float joules)
    {
        return 100 * joules / 2000000;
    }
}
