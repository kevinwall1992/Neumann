using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteAlways]
public class Deposit : MonoBehaviour
{
    [SerializeField]
    protected MeshRenderer extent_mesh_renderer = null;

    [SerializeField]
    Material material = null;

    public string Name = "Deposit";
    public float Volume;
    public Pile Composition;
    public DistributionType Distribution = DistributionType.Gaussian;
    public Color Color = Color.white;
    public bool VisualizeExtent = true;

    public ColorMaterialProperty ColorMaterialProperty;
    public FloatMaterialProperty OpacityMaterialProperty;
    public BoolMaterialProperty IsUniformMaterialProperty;

    //This is a radius
    public float Extent
    {
        get { return transform.localScale.x; }
        set
        {
            transform.localScale = new Vector3(value, value, value);
        }
    }

    protected virtual void Start()
    {
        
    }

    protected virtual void Update()
    {
        extent_mesh_renderer.enabled = VisualizeExtent;

        if (EditorApplication.isPlaying)
        {
            ColorMaterialProperty.Value = Color;
            OpacityMaterialProperty.Value = Mathf.Max(0.05f, Mathf.Min(1, (Mathf.Log10(Volume / (Extent * Extent)) + 1) / 4));
            IsUniformMaterialProperty.Value = Distribution == DistributionType.Uniform;
        }
        else
        {
            transform.localScale = transform.localScale.YYY();

            extent_mesh_renderer.material = material;
        }

    }

    public float GetVolumeWithinRange(Vector3 position, float range, Resource resource = null)
    {
        float distance = position.YChangedTo(0).Distance(transform.position.YChangedTo(0));
        if (distance > Extent)
            return 0;

        float gaussian_weight = Mathf.Exp(-0.5f * Mathf.Pow(distance / Extent * 3, 2)) / Mathf.Sqrt(2 * Mathf.PI);
        float relative_area = (range * range) / (Extent * Extent);
        float volume = Volume;
        if (resource != null)
            volume = Composition.Normalized().GetVolumeOf(resource) * Volume;

        float volume_within_range = relative_area * volume;

        switch (Distribution)
        {
            case DistributionType.Gaussian:
                volume_within_range *= gaussian_weight / unit_gaussian_area;
                break;
        }

        return Mathf.Min(Volume, volume_within_range);
    }


    //integrate 2 * pi * x * e ^ (-0.5 * (x * 3) ^ 2) / (2 * pi) ^ 0.5 from x = 0 to x = 1
    static float unit_gaussian_area = 0.2754f;

    public static Deposit Create()
    {
        Deposit deposit = GameObject.Instantiate(Scene.Main.Prefabs.Deposit);
        deposit.Volume = 0;

        return deposit;
    }


    public enum DistributionType { Gaussian, Uniform }
}


