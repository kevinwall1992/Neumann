using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class MaterialProperty<T>
{
    [SerializeField]
    Renderer renderer = null;
    [SerializeField]
    string name = null;

    protected string Name { get { return name; } }
    protected Material Material { get { return renderer.material; } }

    public abstract T Value { get; set; }
}

[System.Serializable]
public class IntMaterialProperty : MaterialProperty<int>
{
    public override int Value
    {
        get { return Material.GetInt(Name); }
        set { Material.SetInt(Name, value); }
    }
}

[System.Serializable]
public class FloatMaterialProperty : MaterialProperty<float>
{
    public override float Value
    {
        get { return Material.GetFloat(Name); }
        set { Material.SetFloat(Name, value); }
    }
}

[System.Serializable]
public class VectorMaterialProperty : MaterialProperty<Vector4>
{
    public override Vector4 Value
    {
        get { return Material.GetVector(Name); }
        set { Material.SetVector(Name, value); }
    }
}

[System.Serializable]
public class ColorMaterialProperty : MaterialProperty<Color>
{
    public override Color Value
    {
        get { return Material.GetColor(Name); }
        set { Material.SetColor(Name, value); }
    }
}
