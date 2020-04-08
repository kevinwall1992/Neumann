using UnityEngine;
using System.Collections;

public static class Measures
{
    static float JoulesToEnergyUnitsRatio = 10000;

    public static float EnergyUnitsToJoules(float energy_units)
    {
        return energy_units * JoulesToEnergyUnitsRatio;
    }

    public static float JoulesToEnergyUnits(float joules)
    {
        return joules / JoulesToEnergyUnitsRatio;
    }
}
