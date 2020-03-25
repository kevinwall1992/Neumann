using UnityEngine;
using System.Collections;

public class Target
{
    System.Func<Vector3> targeting_function;

    public Mortal Mortal { get; private set; }
    public bool IsMortal { get { return Mortal != null; } }

    public Vector3 Position
    {
        get
        {
            return targeting_function();
        }
    }

    public Target(Vector3 target_position)
    {
        targeting_function = () => target_position;
    }

    public Target(GameObject target)
    {
        targeting_function = () => target.transform.position;

        Mortal = target.GetComponent<Mortal>();
    }

    public static implicit operator Target(Vector3 target_position)
    {
        return new Target(target_position);
    }

    public static implicit operator Target(GameObject target)
    {
        return new Target(target);
    }

    public static implicit operator Target(MonoBehaviour target)
    {
        return new Target(target.gameObject);
    }

    public static Target Convert(object obj)
    {
        if (obj is Target) return (Target)obj;
        else if (obj is Vector3) return (Vector3)obj;
        else if (obj is GameObject) return (GameObject)obj;
        else if (obj is MonoBehaviour) return (MonoBehaviour)obj;
        else return null;
    }
}
