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


    //Band-aid solution for scale issues.
    public static float WorldUnitsToMeters(float world_units)
    {
        return world_units / Scene.Main.World.PhysicalScale;
    }

    public static float MetersToWorldUnits(float meters)
    {
        return meters * Scene.Main.World.PhysicalScale;
    }
}
