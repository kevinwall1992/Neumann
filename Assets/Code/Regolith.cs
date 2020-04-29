using UnityEngine;
using System.Collections;

public class Regolith : Stratum
{
    bool IsOccluded(Vector3 position, float range)
    {
        return Scene.Main.World.Asteroid.Surface
            .GetSupplyWithinRange(position, range).Volume > 0;
    }

    public override Pile TakeSample(Vector3 position, float range, float volume)
    {
        if (IsOccluded(position, range))
            return Scene.Main.World.Asteroid.Surface
                .TakeSample(position, range, volume);

        return base.TakeSample(position, range, volume);
    }
}
