using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Toggler))]
public class SolarCollector : Profession
{
    public float Efficiency;
    public float OptimalFlux;
    public float HalfFlux;

    public float EnergyPerSecond
    {
        get
        {
            float flux = Scene.Main.World.Asteroid.GetSolarFlux(Unit.Physical.Position);
            float flux_efficiency = Mathf.Pow(0.5f,
                (flux - OptimalFlux) /
                (HalfFlux - OptimalFlux));
            if (flux_efficiency > 1)
                flux_efficiency = 1 + (flux_efficiency - 1) / 4;

            float total_efficiency = Mathf.Min(1, Efficiency * flux_efficiency);
            float area = Mathf.Pow(Measures.WorldUnitsToMeters(Unit.Physical.Size * 2), 2);

            return Measures.JoulesToEnergyUnits(total_efficiency * flux * area);
        }
    }

    public override IEnumerable<Operation> Abilities { get { return new List<Operation>(); } }

    public Toggler Toggler { get { return GetComponent<Toggler>(); } }

    void Start()
    {

    }

    void Update()
    {
        if (Toggler.IsOn)
            Unit.Team.Stock.Pile.PutIn(Resource.Energy, EnergyPerSecond * Time.deltaTime);
    }
}
