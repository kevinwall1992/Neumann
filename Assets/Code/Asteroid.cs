using UnityEngine;
using System.Collections;

public class Asteroid : CelestialBody
{
    [SerializeField]
    Stratum regolith = null, rock = null;

    public Stratum Regolith { get { return regolith; } }
    public Stratum Rock { get { return rock; } }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
