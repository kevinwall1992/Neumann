﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Resource
{
    public string Name;

    public Sprite Icon
    {
        get { return Scene.Main.Style.Variables[Name].Sprite; }
    }

    public Color Color
    {
        get { return Scene.Main.Style.Variables[Name].Color; }
    }

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


    public static Resource Energy { get { return new Resource("Energy"); } }
    public static Resource Tools { get { return new Resource("Tools"); } }
}