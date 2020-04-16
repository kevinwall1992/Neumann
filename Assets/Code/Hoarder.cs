using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Unit))]
public class Hoarder : MonoBehaviour
{
    public Pile Capacity { get; private set; } = new Pile();
    public Pile Contents { get; private set; } = new Pile();

    public Unit Unit { get { return GetComponent<Unit>(); } }

    private void Update()
    {
        foreach (Resource resource in new List<Resource>(Contents.Resources))
        {
            float volume = Contents.GetVolumeOf(resource);
            float capacity = Capacity.GetVolumeOf(resource);

            if (volume > capacity)
                Unit.Team.Stock.Pile.PutIn(Contents.TakeOut(resource, volume - capacity));
        }
    }
}
