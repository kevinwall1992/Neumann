using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Unit))]
public abstract class Able : MonoBehaviour, BehaviorController
{
    public abstract IEnumerable<Task> Abilities { get; }

    public Unit Unit { get { return GetComponent<Unit>(); } }
    public Task Task { get { return Unit.Task; } }

    public GameObject BehaviorHost { get { return gameObject; } }
}
