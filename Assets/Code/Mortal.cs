using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mortal : MonoBehaviour, HasVariables
{
    public FillableAttribute Health = new FillableAttribute("Health", 100);

    //Fraction of impact energy that results in damage
    public float BaseImpactFragility;
    //Maximum impact velocity before suffering damage
    public float BaseImpactTolerance;
    //Fraction of damage taken from radiation
    public float BaseRadiationHardness;

    public bool IsDead { get { return Health.Value <= 0; } }

    public List<Variable> Variables { get { return Health.Variables; } }

    public float ImpactFragility { get { return BaseImpactFragility; } }
    public float ImpactTolerance { get { return BaseImpactTolerance; } }
    public float RadiationHardness { get { return BaseRadiationHardness; } }

    void Start()
    {
        
    }

    void Update()
    {

    }
}
