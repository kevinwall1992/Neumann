using UnityEngine;
using System.Collections;

public class Asteroid : CelestialBody
{
    [SerializeField]
    Regolith regolith = null;

    [SerializeField]
    Stratum rock = null;

    public Regolith Regolith { get { return regolith; } }
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
