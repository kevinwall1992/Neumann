using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Stratum : MonoBehaviour
{
    public string Name = "Stratum";

    public IEnumerable<Deposit> Deposits { get { return GetComponentsInChildren<Deposit>();} }

    void Start()
    {

    }

    void Update()
    {

    }

    public virtual float GetVolumeWithinRange(Vector3 position, float range, Resource resource = null)
    {
        return Deposits.Sum(deposit => deposit.GetVolumeWithinRange(position, range, resource));
    }

    public virtual float GetConcentrationByVolume(Vector3 position, float range, Resource resource)
    {
        return GetVolumeWithinRange(position, range, resource) / 
               GetVolumeWithinRange(position, range);
    }

    public virtual Pile TakeSample(Vector3 position, float range, float volume)
    {
        float volume_within_range = GetVolumeWithinRange(position, range);
        if (volume_within_range == 0)
            return new Pile();

        float fraction = Mathf.Min(1, volume / volume_within_range);

        Pile sample = new Pile();
        foreach (Deposit deposit in Deposits)
        {
            float volume_removed = deposit.GetVolumeWithinRange(position, range) * fraction;

            deposit.Volume -= volume_removed;
            sample.PutIn(deposit.Composition.Normalized() * volume_removed);
        }

        return sample;
    }
}
