using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Behavior : MonoBehaviour, BehaviorController
{
    public GameObject BehaviorHost { get { return gameObject; } }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {

    }

    internal virtual void CleanUp()
    {

    }

    protected virtual void OnDestroy()
    {
        CleanUp();
    }
}


public interface BehaviorController
{
    GameObject BehaviorHost { get; }
}

public static class BehaviorControllerExtensions
{
    static Dictionary<BehaviorController, Dictionary<System.Type, Behavior>> behavior_map = 
        new Dictionary<BehaviorController, Dictionary<System.Type, Behavior>>();

    public static T Start<T>(this BehaviorController behavior_controller) where T : Behavior
    {
        GameObject host = behavior_controller.BehaviorHost;

        T behavior = host.GetComponent<T>();
        if (behavior == null)
            behavior = host.AddComponent<T>();

        if (!behavior_map.ContainsKey(behavior_controller))
            behavior_map[behavior_controller] = new Dictionary<System.Type, Behavior>();
        behavior_map[behavior_controller][typeof(T)] = behavior;

        return behavior;
    }

    public static T GetBehavior<T>(this BehaviorController behavior_controller) where T : Behavior
    {
        if (!behavior_map.ContainsKey(behavior_controller) ||
            !behavior_map[behavior_controller].ContainsKey(typeof(T)))
            return null;

        return behavior_map[behavior_controller][typeof(T)] as T;
    }

    public static void Stop<T>(this BehaviorController behavior_controller) where T : Behavior
    {
        Behavior behavior = behavior_controller.GetBehavior<T>();
        if (behavior == null)
            return;

        behavior.CleanUp();
        GameObject.Destroy(behavior);
        behavior_map[behavior_controller][typeof(T)] = null;
    }
}
