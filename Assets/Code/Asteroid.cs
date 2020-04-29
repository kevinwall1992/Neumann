using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Asteroid : CelestialBody
{
    [SerializeField]
    Regolith regolith = null;

    [SerializeField]
    Stratum rock = null;

    public Regolith Regolith { get { return regolith; } }
    public Stratum Rock { get { return rock; } }

    public override string Name { get { return "Asteroid"; } }

    public override IEnumerable<InfoBox.Info> Infos
    {
        get
        {
            return Regolith.GetSupplyWithinRange(
                GetWorldPositionPointedAt(), 
                Loader.GetRange(100))
                .Infos;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
