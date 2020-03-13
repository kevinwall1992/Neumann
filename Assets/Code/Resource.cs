using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Resource
{
    public string Name;

    public Resource(string name)
    {
        Name = name;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj))
            return true;

        Resource other = obj as Resource;
        if (other == null)
            return false;

        return Name == other.Name;
    }

    public override int GetHashCode()
    {
        return 1;
    }
}