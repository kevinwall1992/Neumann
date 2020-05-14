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

    public HighwaySystem HighwaySystem { get; private set; }

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

        IEnumerable<Vector3> deposit_positions = Regolith.Deposits.Merged(Rock.Deposits)
            .Where(deposit => deposit.Distribution != Deposit.DistributionType.Uniform)
            .Select(deposit => deposit.transform.position);
        deposit_positions = deposit_positions
            .Select(position => position.YChangedTo(GetSurfaceHeight(position)));

        HighwaySystem = new HighwaySystem(Terrain, deposit_positions);
    }

    protected override void Update()
    {
        base.Update();
    }
}
