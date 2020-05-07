using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UIUtility
{
    //Like [RequireComponent], but at runtime
    public static T RequireComponent<T>(this GameObject game_object) where T : MonoBehaviour
    {
        T component = game_object.GetComponent<T>();
        if (component == null)
            component = game_object.AddComponent<T>();

        return component;
    }

    public static T RequireComponent<T>(this MonoBehaviour mono_behaviour) where T : MonoBehaviour
    {
        return RequireComponent<T>(mono_behaviour.gameObject);
    }

    static void SetLayer(this GameObject game_object, int layer_index)
    {
        game_object.layer = layer_index;

        foreach (Transform child_transform in game_object.transform)
            child_transform.gameObject.SetLayer(layer_index);
    }

    public static void SetLayer(this GameObject game_object, string layer_name)
    {
        game_object.SetLayer(LayerMask.NameToLayer(layer_name));
    }


    public static Transform FindDescendent(this Transform transform, string name)
    {
        Queue<Transform> descendents = new Queue<Transform>();
        foreach (Transform child in transform)
            descendents.Enqueue(child);

        while (descendents.Count > 0)
        {
            Transform descendent = descendents.Dequeue();

            if (descendent.name == name)
                return descendent;

            foreach (Transform child in descendent.transform)
                descendents.Enqueue(child);
        }

        return null;
    }

    public static T FindDescendent<T>(this Transform transform, string name) where T : MonoBehaviour
    {
        return transform.FindDescendent(name).GetComponent<T>();
    }

    public static Transform FindAncestor(this Transform transform, string name)
    {
        Transform ancestor = transform.parent;

        while (ancestor != null)
        {
            if (ancestor.name == name)
                return ancestor;

            ancestor = ancestor.parent;
        }

        return null;
    }

    public static T FindAncestor<T>(this Transform transform, string name) where T : MonoBehaviour
    {
        return transform.FindAncestor(name).GetComponent<T>();
    }

    public static bool HasComponent<T>(this GameObject game_object)
    {
        return game_object.GetComponent<T>() != null;
    }

    public static bool HasComponent<T>(this MonoBehaviour mono_behaviour)
    {
        return mono_behaviour.gameObject.HasComponent<T>();
    }

    public static bool HasComponent<T>(this Transform transform)
    {
        return transform.gameObject.HasComponent<T>();
    }

    //A transform is considered to descend from itself
    public static bool DescendsFrom(this Transform descendent, Transform ancestor)
    {
        Transform transform = descendent;

        while (transform != null)
        {
            if (transform == ancestor)
                return true;

            transform = transform.parent;
        }

        return false;
    }

    public static bool DescendsFrom(this MonoBehaviour descendent, Transform ancestor)
    { return DescendsFrom(descendent.transform, ancestor); }
    public static bool DescendsFrom(this MonoBehaviour descendent, MonoBehaviour ancestor)
    { return DescendsFrom(descendent.transform, ancestor.transform); }
    public static bool DescendsFrom(this GameObject descendent, Transform ancestor)
    { return DescendsFrom(descendent.transform, ancestor); }
    public static bool DescendsFrom(this GameObject descendent, GameObject ancestor)
    { return DescendsFrom(descendent.transform, ancestor.transform); }

    public static bool IsModulusUpdate(this MonoBehaviour mono_behaviour, int divisor)
    {
        if (!update_counts.ContainsKey(mono_behaviour))
            update_counts[mono_behaviour] = 0;

        return update_counts[mono_behaviour]++ % divisor == 0;
    }
    static Dictionary<MonoBehaviour, int> update_counts = new Dictionary<MonoBehaviour, int>();

    public static bool Contains(this RectTransform rect_transform, Vector2 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect_transform, position);
    }
}
