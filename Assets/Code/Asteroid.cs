using UnityEngine;
using System.Collections;

public class Asteroid : CelestialBody
{
    [SerializeField]
    Stratum surface = null, regolith = null, rock = null;

    public Stratum Surface { get { return surface; } }
    public Stratum Regolith { get { return regolith; } }
    public Stratum Rock { get { return rock; } }

    void Start()
    {

    }

    void Update()
    {

    }
}
