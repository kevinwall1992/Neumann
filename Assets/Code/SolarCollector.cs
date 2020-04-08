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

            float area = Mathf.Pow((Unit.Physical.Size * 2), 2);

            return Measures.JoulesToEnergyUnits(Efficiency * flux_efficiency * flux * area);
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
