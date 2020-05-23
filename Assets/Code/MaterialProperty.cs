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

    public T GetValueOfOther(Renderer other_renderer)
    {
        Renderer this_renderer = this.renderer;
        this.renderer = other_renderer;

        T value = Value;

        this.renderer = this_renderer;

        return value;
    }

    public void SetValueOfOther(Renderer other_renderer, T value)
    {
        Renderer this_renderer = this.renderer;
        this.renderer = other_renderer;

        Value = value;

        this.renderer = this_renderer;
    }
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

[System.Serializable]
public class BoolMaterialProperty : MaterialProperty<bool>
{
    public override bool Value
    {
        get { return Material.GetInt(Name) != 0; }
        set { Material.SetInt(Name, value ? 1 : 0); }
    }
}