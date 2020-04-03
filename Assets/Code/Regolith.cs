using UnityEngine;
using System.Collections;

public class Regolith : Stratum
{
    bool IsOccluded(Vector3 position, float range)
    {
        return Scene.Main.World.Asteroid.Surface
            .GetVolumeWithinRange(position, range) > 0;
    }

    public override float GetVolumeWithinRange(Vector3 position, float range, Resource resource = null)
    {
        if (IsOccluded(position, range))
            return Scene.Main.World.Asteroid.Surface
                .GetVolumeWithinRange(position, range, resource);

        return base.GetVolumeWithinRange(position, range, resource);
    }

    public override float GetConcentrationByVolume(Vector3 position, float range, Resource resource)
    {
        if (IsOccluded(position, range))
            return Scene.Main.World.Asteroid.Surface
                .GetConcentrationByVolume(position, range, resource);

        return base.GetConcentrationByVolume(position, range, resource);
    }

    public override Pile TakeSample(Vector3 position, float range, float volume)
    {
        if (IsOccluded(position, range))
            return Scene.Main.World.Asteroid.Surface
                .TakeSample(position, range, volume);

        return base.TakeSample(position, range, volume);
    }
}
