﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using RotaryHeart.Lib.SerializableDictionary;


[System.Serializable]
public class Pile : InfoBox.HasInfos
{
    [System.Serializable]
    class ResourceQuantityMap : SerializableDictionaryBase<Resource, float> { }
    [SerializeField]
    ResourceQuantityMap resources = new ResourceQuantityMap();

    public IEnumerable<Resource> Resources
    {
        get { return resources.Keys; }
    }

    public float Volume
    {
        get
        {
            return resources.Values.Sum();
        }

        set
        {
            if (value < Volume)
                TakeSlice(1 - value / Volume);
            else
                PutIn(Normalized() * (value - Volume));
        }
    }

    string InfoBox.HasInfos.Name { get { return "Pile"; } }

    public IEnumerable<InfoBox.Info> Infos
    {
        get
        {
            return Resources
                .Sorted(resource => GetVolumeOf(resource))
                .Reversed().GetRange_NoExcuses(0, 3)
                .Select(resource => new InfoBox.Info(
                    resource.Name,
                    GetVolumeOf(resource).ToString("F0") +
                        " (" + (Normalized().GetVolumeOf(resource) * 100).ToString("F0") + "%)"));
        }
    }

    public Pile()
    {
        
    }

    void Require(Resource resource)
    {
        if (!resources.ContainsKey(resource))
            resources[resource] = 0;
    }

    public Pile PutIn(Resource resource, float volume)
    {
        Require(resource);

        resources[resource] += volume;

        return this;
    }

    public Pile PutIn(Pile pile)
    {
        foreach (Resource resource in pile.Resources)
            PutIn(resource, pile.GetVolumeOf(resource));

        return this;
    }

    public Pile TakeOut(Resource resource, float volume = float.MaxValue)
    {
        Require(resource);

        volume = Mathf.Min(volume, GetVolumeOf(resource));
        resources[resource] -= volume;

        return new Pile().PutIn(resource, volume);
    }

    public Pile TakeOut(Pile pile)
    {
        //This preserves ratios of resources
        float fraction = pile.Resources.Min(delegate (Resource resource) 
        {
            if (pile.GetVolumeOf(resource) == 0)
                return 1;

            return Mathf.Min(1, GetVolumeOf(resource) / pile.GetVolumeOf(resource));
        });

        Pile removed_pile = new Pile();
        foreach (Resource resource in pile.Resources)
            removed_pile.PutIn(TakeOut(resource, pile.GetVolumeOf(resource) * fraction));

        return removed_pile;
    }

    public Pile TakeOut(float volume)
    {
        return TakeOut(Normalized() * volume);
    }

    public Pile TakeSlice(float fraction)
    {
        return TakeOut(Volume * fraction);
    }

    public Pile TakeAll()
    {
        return TakeSlice(1);
    }

    public float GetVolumeOf(Resource resource)
    {
        if (!Resources.Contains(resource))
            return 0;

        return resources[resource];
    }

    public Pile Normalized()
    {
        return this / Volume;
    }

    public Resource GetPrincipleComponent()
    {
        if (resources.Count == 0)
            return null;

        return resources.Keys.Sorted(resource => GetVolumeOf(resource)).Last();
    }

    public Pile Copy()
    {
        Pile pile = new Pile();

        foreach (Resource resource in Resources)
            pile.PutIn(resource, GetVolumeOf(resource));

        return pile;
    }


    public static Pile operator *(Pile pile, float scalar)
    {
        Pile scaled_pile = pile.Copy();
        foreach (Resource resource in new List<Resource>(pile.Resources))
            scaled_pile.resources[resource] *= scalar;

        return scaled_pile;
    }

    public static Pile operator /(Pile pile, float scalar)
    {
        return pile * (1 / scalar);
    }

    public static Pile operator +(Pile a, Pile b)
    {
        Pile sum = a.Copy();
        sum.PutIn(b);

        return sum;
    }
}

