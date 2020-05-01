using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;


public static class Utility
{
    public static List<T> List<T>(params T[] elements)
    {
        return new List<T>(elements);
    }

    public static Dictionary<T, U> Dictionary<T, U>(params object[] keys_and_values)
    {
        Dictionary<T, U> dictionary = new Dictionary<T, U>();

        for (int i = 0; i < keys_and_values.Length / 2; i++)
        {
            T t = (T)keys_and_values[i * 2 + 0];
            U u = (U)keys_and_values[i * 2 + 1];

            if (t != null && u != null)
                dictionary[t] = u;
        }

        return dictionary;
    }

    public static List<T> CreateNullList<T>(int size) where T : class
    {
        List<T> list = new List<T>();

        for (int i = 0; i < size; i++)
            list.Add(null);

        return list;
    }

    public static bool SetEquality<T>(this IEnumerable<T> a, IEnumerable<T> b)
    {
        return a.Count() == b.Count() && a.Except(b).Count() == 0;
    }

    public static bool SetEquality<T, U>(this Dictionary<T, U> a, Dictionary<T, U> b)
    {
        return a.Count == b.Count && a.Except(b).Count() == 0;
    }

    public static T Take<T>(this List<T> list, T element)
    {
        list.Remove(element);

        return element;
    }

    public static T TakeAt<T>(this List<T> list, int index)
    {
        T element = list[index];
        list.RemoveAt(index);

        return element;
    }

    public static List<T> GetRange_NoExcuses<T>(this List<T> list, int index, int count)
    {
        if (index < 0 || index >= list.Count)
            return new List<T>();

        return list.GetRange(index, Mathf.Min(count, list.Count - index));
    }

    public static List<T> Reversed<T>(this List<T> list)
    {
        List<T> reversed = new List<T>(list);
        reversed.Reverse();

        return reversed;
    }

    public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
    {
        foreach (T element in enumerable)
            action(element);
    }

    public static IEnumerable<T> Merged<T>(this IEnumerable<T> enumerable, IEnumerable<T> other)
    {
        List<T> merged = new List<T>(enumerable);
        merged.AddRange(other);

        return merged;
    }

    public static List<T> Merged<T>(this List<T> list, IEnumerable<T> other)
    {
        return (list as IEnumerable<T>).Merged(other) as List<T>;
    }

    public static Dictionary<T, U> Merged<T, U>(this Dictionary<T, U> a, Dictionary<T, U> b)
    {
        Dictionary<T, U> merged = new Dictionary<T, U>(a);

        foreach (T key in b.Keys)
            merged[key] = b[key];

        return merged;
    }

    public static string Trim(this string string_, int trim_count)
    {
        return string_.Substring(0, string_.Length - trim_count);
    }

    public static List<T> Sorted<T, U>(this List<T> list, Func<T, U> comparable_fetcher) where U : IComparable
    {
        List<T> sorted = new List<T>(list);
        sorted.Sort((a, b) => (comparable_fetcher(a).CompareTo(comparable_fetcher(b))));

        return sorted;
    }

    public static List<T> Sorted<T, U>(this IEnumerable<T> enumerable, Func<T, U> comparable_fetcher) where U : IComparable
    {
        return Sorted(new List<T>(enumerable), comparable_fetcher);
    }

    public static T MinElement<T, U>(this IEnumerable<T> enumerable, Func<T, U> comparable_fetcher) where U : IComparable
    {
        return enumerable.Sorted(comparable_fetcher).First();
    }

    public static T MaxElement<T, U>(this IEnumerable<T> enumerable, Func<T, U> comparable_fetcher) where U : IComparable
    {
        return enumerable.Sorted(comparable_fetcher).Last();
    }

    public static int DuplicateCountOf<T>(this IEnumerable<T> enumerable, T element)
    {
        return enumerable.Sum(other_element => (EqualityComparer<T>.Default.Equals(other_element, element) ? 1 : 0));
    }

    public static List<T> RemoveDuplicates<T>(this IEnumerable<T> enumerable)
    {
        List<T> without_duplicates = new List<T>();

        foreach (T element in enumerable)
            if (!without_duplicates.Contains(element))
                without_duplicates.Add(element);

        return without_duplicates;
    }

    public static System.Func<T, U> CreateLookup<T, U>(this System.Func<T, U> Function, IEnumerable<T> domain)
    {
        Dictionary<T, U> dictionary = new Dictionary<T, U>();

        foreach (T t in domain)
            dictionary[t] = Function(t);

        return (t) => (dictionary[t]);
    }

    public static System.Func<U, T> CreateInverseLookup<T, U>(this System.Func<T, U> Function, IEnumerable<T> domain)
    {
        Dictionary<U, T> inverse_dictionary = new Dictionary<U, T>();

        foreach (T t in domain) 
            inverse_dictionary[Function(t)] = t;

        return (u) => (inverse_dictionary[u]);
    }

    public static List<U> ToParentType<T, U>(this IEnumerable<T> enumerable) where T : U
    {
        return (new List<T>(enumerable)).ConvertAll(t => (U)t);
    }

    public static List<U> ToChildType<T, U>(this IEnumerable<T> enumerable) where U : T
    {
        return (new List<T>(enumerable)).ConvertAll(t => (U)t);
    }


    public static IEnumerable<T> GetEnumValues<T>()
    {
        if (!typeof(T).IsEnum)
            throw new ArgumentException("Type T must be an Enum");

        return (T[])System.Enum.GetValues(typeof(T));
    }

    public static bool IsInPast(this DateTime time)
    {
        return DateTime.Now > time;
    }

    public static bool IsInFuture(this DateTime time)
    {
        return DateTime.Now < time;
    }

    public static float GetCycleMoment(float cycle_length, DateTime start)
    {
        float cycles = (float)(DateTime.Now - start).TotalSeconds / cycle_length;

        return cycles - (int)cycles;
    }

    public static float GetCycleMoment(float cycle_length)
    {
        return GetCycleMoment(cycle_length, LevelLoadDateTime);
    }

    public static float GetLoopedCycleMoment(float cycle_length, DateTime start)
    {
        float moment = GetCycleMoment(cycle_length, start);
        if (moment > 0.5f)
            moment = 0.5f - (moment - 0.5f);

        return moment * 2;
    }

    public static float GetLoopedCycleMoment(float cycle_length)
    {
        return GetLoopedCycleMoment(cycle_length, LevelLoadDateTime);
    }

    public static DateTime LevelLoadDateTime
    { get { return DateTime.Now.AddSeconds(-Time.timeSinceLevelLoad); } }
}

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


    public static Color AlphaChangedTo(this Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
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

    public static Color Lerped(this Color color, Color other, float factor)
    {
        return Color.Lerp(color, other, factor);
    }

    public static bool IsModulusUpdate(this MonoBehaviour mono_behaviour, int divisor)
    {
        if (!update_counts.ContainsKey(mono_behaviour))
            update_counts[mono_behaviour] = 0;

        return update_counts[mono_behaviour]++ % divisor == 0;
    }
    static Dictionary<MonoBehaviour, int> update_counts = new Dictionary<MonoBehaviour, int>();
}

public static class InputUtility
{
    public enum MouseButton { Left, Right, Middle }

    public static bool WasMouseLeftPressed() { return Input.GetMouseButtonDown((int)MouseButton.Left); }
    public static bool IsMouseLeftPressed() { return Input.GetMouseButton((int)MouseButton.Left); }
    public static bool WasMouseLeftReleased() { return Input.GetMouseButtonUp((int)MouseButton.Left); }

    public static bool WasMouseRightPressed() { return Input.GetMouseButtonDown((int)MouseButton.Right); }
    public static bool IsMouseRightPressed() { return Input.GetMouseButton((int)MouseButton.Right); }
    public static bool WasMouseRightReleased() { return Input.GetMouseButtonUp((int)MouseButton.Right); }

    public static bool IsDragOccurring() { return Scene.Main.InputModule.IsDragOccurring; }
    public static bool DidDragOccur() { return Scene.Main.InputModule.DidDragOccur; }

    public static bool DidMouseMove() { return Scene.Main.InputModule.DidMouseMove; }


    static MonoBehaviour keyboard_input_claimant = null;
    public static bool ClaimKeyboardInput(this MonoBehaviour claimant)
    {
        if (!claimant.CanUseKeyboardInput())
            return false;

        keyboard_input_claimant = claimant;
        return true;
    }

    public static void YieldKeyboardInputClaim(this MonoBehaviour claimant)
    {
        if (keyboard_input_claimant == claimant)
            keyboard_input_claimant = null;
    }

    public static bool CanUseKeyboardInput(this MonoBehaviour enquirer)
    {
        return keyboard_input_claimant == null || 
               keyboard_input_claimant == enquirer;
    }


    static MonoBehaviour mouse_left_release_claimant = null;
    static int mouse_left_release_claim_yield_frame = -1;
    static MonoBehaviour MouseLeftReleaseClaimant
    {
        get
        {
            if (mouse_left_release_claim_yield_frame >= 0 &&
               mouse_left_release_claim_yield_frame < Time.frameCount)
                mouse_left_release_claimant = null;

            return mouse_left_release_claimant;
        }

        set
        {
            if (mouse_left_release_claimant != null ||
                mouse_left_release_claimant == value)
            {
                mouse_left_release_claimant = value;
                mouse_left_release_claim_yield_frame = -1;
            }
        }
    }

    public static void ClaimMouseLeftRelease(this MonoBehaviour claimant)
    {
        MouseLeftReleaseClaimant = claimant;
    }

    public static void YieldMouseLeftReleaseClaim(this MonoBehaviour claimant)
    {
        if (MouseLeftReleaseClaimant = claimant)
            mouse_left_release_claim_yield_frame = Time.frameCount;
    }

    public static bool UseMouseLeftRelease(this MonoBehaviour enquirer)
    {
        if (!WasMouseLeftReleased())
            return false;

        if (MouseLeftReleaseClaimant != null)
        {
            if (MouseLeftReleaseClaimant == enquirer)
            {
                YieldMouseLeftReleaseClaim(enquirer);
                return true;
            }

            return false;
        }

        return true;
    }

    public static Vector2 GetMouseMotion()
    {
        return new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
    }

    public static bool IsPointedAt(this GameObject game_object)
    {
        if (Scene.Main.InputModule.IsHovered(game_object))
            return true;

        if (game_object.transform is RectTransform)
            return (game_object.transform as RectTransform).ContainsMouse();

        Collider collider = game_object.GetComponent<SphereCollider>();
        if (collider != null)
        {
            RaycastHit hit_info;
            return collider.Raycast(Scene.Main.Camera.ScreenPointToRay(Input.mousePosition), 
                                    out hit_info, 
                                    10000);
        }

        return false;
    }

    public static bool IsPointedAt(this MonoBehaviour mono_behaviour)
    {
        return mono_behaviour.gameObject.IsPointedAt();
    }

    public static bool IsPointedAt(this Transform transform)
    {
        return transform.gameObject.IsPointedAt();
    }

    public static T GetElementTouched<T>() where T : class
    {
        if (Scene.Main.InputModule.ElementTouched == null)
            return null;

        return Scene.Main.InputModule.ElementTouched.GetComponentInParent<T>();
    }

    public static bool IsTouched(this GameObject game_object)
    {
        if (Scene.Main.InputModule.ElementTouched == null)
            return false;

        return Scene.Main.InputModule.ElementTouched.DescendsFrom(game_object);
    }

    public static bool IsTouched(this MonoBehaviour mono_behaviour)
    {
        return mono_behaviour.gameObject.IsTouched();
    }

    public static bool IsTouched(this Transform transform)
    {
        return transform.gameObject.IsTouched();
    }

    public static bool Contains(this RectTransform rect_transform, Vector2 position)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(rect_transform, position);
    }

    public static bool ContainsMouse(this RectTransform rect_transform)
    {
        return rect_transform.Contains(Input.mousePosition);
    }


    //This should only be called from OnGUI()
    public static bool ConsumeIsKeyUp(KeyCode key_code)
    {
        if (Event.current.isKey && Event.current.keyCode == key_code && Input.GetKeyUp(key_code))
        {
            Event.current.Use();

            return true;
        }

        return false;
    }
}

public static class VectorUtility
{
    public static Vector3 InPlane(this Vector3 vector, Vector3 normal)
    {
        return vector - normal * vector.Dot(normal.normalized);
    }

    public static Vector3 InAxis(this Vector3 vector, Vector3 axis)
    {
        return axis * vector.Dot(axis.normalized);
    }

    public static Vector3 Scale(this Vector3 vector, float scalar)
    {
        vector.x *= scalar;
        vector.y *= scalar;
        vector.z *= scalar;

        return vector;
    }

    public static float AngleBetween(this Vector3 vector, Vector3 other)
    {
        return MathUtility.DegreesToRadians(Vector3.Angle(vector, other));
    }

    public static float Distance(this Vector3 vector, Vector3 other) { return Vector3.Distance(vector, other); }
    public static float Dot(this Vector3 vector, Vector3 other) { return Vector3.Dot(vector, other); }
    public static Vector3 Scaled(this Vector3 vector, Vector3 other) { return Vector3.Scale(vector, other); }
    public static Vector3 Crossed(this Vector3 vector, Vector3 other) { return Vector3.Cross(vector, other); }
    public static Vector3 Lerped(this Vector3 vector, Vector3 other, float factor) { return Vector3.Lerp(vector, other, factor); }

    public static Vector3 XChangedTo(this Vector3 vector, float x) { return new Vector3(x, vector.y, vector.z); }
    public static Vector3 YChangedTo(this Vector3 vector, float y) { return new Vector3(vector.x, y, vector.z); }
    public static Vector3 ZChangedTo(this Vector3 vector, float z) { return new Vector3(vector.x, vector.y, z); }

    public static Vector3 XXX(this Vector3 vector) { return new Vector3(vector.x, vector.x, vector.x); }
    public static Vector3 XXY(this Vector3 vector) { return new Vector3(vector.x, vector.x, vector.y); }
    public static Vector3 XXZ(this Vector3 vector) { return new Vector3(vector.x, vector.x, vector.z); }
    public static Vector3 XYX(this Vector3 vector) { return new Vector3(vector.x, vector.y, vector.x); }
    public static Vector3 XYY(this Vector3 vector) { return new Vector3(vector.x, vector.y, vector.y); }
    public static Vector3 XYZ(this Vector3 vector) { return new Vector3(vector.x, vector.y, vector.z); }
    public static Vector3 XZX(this Vector3 vector) { return new Vector3(vector.x, vector.z, vector.x); }
    public static Vector3 XZY(this Vector3 vector) { return new Vector3(vector.x, vector.z, vector.y); }
    public static Vector3 XZZ(this Vector3 vector) { return new Vector3(vector.x, vector.z, vector.z); }

    public static Vector3 YXX(this Vector3 vector) { return new Vector3(vector.y, vector.x, vector.x); }
    public static Vector3 YXY(this Vector3 vector) { return new Vector3(vector.y, vector.x, vector.y); }
    public static Vector3 YXZ(this Vector3 vector) { return new Vector3(vector.y, vector.x, vector.z); }
    public static Vector3 YYX(this Vector3 vector) { return new Vector3(vector.y, vector.y, vector.x); }
    public static Vector3 YYY(this Vector3 vector) { return new Vector3(vector.y, vector.y, vector.y); }
    public static Vector3 YYZ(this Vector3 vector) { return new Vector3(vector.y, vector.y, vector.z); }
    public static Vector3 YZX(this Vector3 vector) { return new Vector3(vector.y, vector.z, vector.x); }
    public static Vector3 YZY(this Vector3 vector) { return new Vector3(vector.y, vector.z, vector.y); }
    public static Vector3 YZZ(this Vector3 vector) { return new Vector3(vector.y, vector.z, vector.z); }

    public static Vector3 ZXX(this Vector3 vector) { return new Vector3(vector.z, vector.x, vector.x); }
    public static Vector3 ZXY(this Vector3 vector) { return new Vector3(vector.z, vector.x, vector.y); }
    public static Vector3 ZXZ(this Vector3 vector) { return new Vector3(vector.z, vector.x, vector.z); }
    public static Vector3 ZYX(this Vector3 vector) { return new Vector3(vector.z, vector.y, vector.x); }
    public static Vector3 ZYY(this Vector3 vector) { return new Vector3(vector.z, vector.y, vector.y); }
    public static Vector3 ZYZ(this Vector3 vector) { return new Vector3(vector.z, vector.y, vector.z); }
    public static Vector3 ZZX(this Vector3 vector) { return new Vector3(vector.z, vector.z, vector.x); }
    public static Vector3 ZZY(this Vector3 vector) { return new Vector3(vector.z, vector.z, vector.y); }
    public static Vector3 ZZZ(this Vector3 vector) { return new Vector3(vector.z, vector.z, vector.z); }

    public static Vector2 XX(this Vector3 vector) { return new Vector2(vector.x, vector.y); }
    public static Vector2 XY(this Vector3 vector) { return new Vector2(vector.x, vector.z); }
    public static Vector2 XZ(this Vector3 vector) { return new Vector2(vector.y, vector.x); }
    public static Vector2 YX(this Vector3 vector) { return new Vector2(vector.y, vector.z); }
    public static Vector2 YY(this Vector3 vector) { return new Vector2(vector.z, vector.x); }
    public static Vector2 YZ(this Vector3 vector) { return new Vector2(vector.z, vector.y); }
    public static Vector2 ZX(this Vector3 vector) { return new Vector2(vector.y, vector.z); }
    public static Vector2 ZY(this Vector3 vector) { return new Vector2(vector.z, vector.x); }
    public static Vector2 ZZ(this Vector3 vector) { return new Vector2(vector.z, vector.y); }
}
