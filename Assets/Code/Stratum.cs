﻿using UnityEngine;
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

    public Deposit GetNearestOverlappingDeposit(Vector3 position)
    {
        IEnumerable<Deposit> overlapping_deposits = 
            Deposits.Where(deposit => deposit.transform.position.Distance(position) <= 
                                      deposit.Extent);

        if (overlapping_deposits.Count() == 0)
            return null;

        return overlapping_deposits
            .Sorted(deposit => deposit.transform.position.Distance(position))
            .First();
    }

    public virtual Pile GetSupplyWithinRange(Vector3 position, float range, Resource resource = null)
    {
        return Deposits.Aggregate(new Pile(), (pile, deposit) => pile
            .PutIn(deposit.GetSupplyWithinRange(position, range, resource)));
    }

    public virtual Pile TakeSample(Vector3 position, float range, float volume)
    {
        if (volume <= 0)
            return new Pile();

        float volume_within_range = GetSupplyWithinRange(position, range).Volume;
        if (volume_within_range == 0)
            return new Pile();

        float fraction = Mathf.Min(1, volume / volume_within_range);

        Pile sample = new Pile();
        foreach (Deposit deposit in new List<Deposit>(Deposits))
        {
            float volume_removed = deposit.GetSupplyWithinRange(position, range).Volume * fraction;

            deposit.Volume -= volume_removed;
            if(deposit.Volume < 0.000001f)
            {
                deposit.transform.SetParent(null);
                GameObject.Destroy(deposit.gameObject);
            }

            sample.PutIn(deposit.Composition.Normalized() * volume_removed);
        }

        return sample;
    }
}
