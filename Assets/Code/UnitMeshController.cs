using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MaterialSprayer))]
public class UnitMeshController : MonoBehaviour
{
    Material default_material_instance;
    Material buildable_material_instance;

    public Unit Unit;
    public float BaseHeight = 1;
    public Material DefaultMaterial;
    public Material BuildableMaterial;
    public Texture Texture;
    public string ColorName;
    public string YMaskName;

    public MaterialSprayer MaterialSprayer { get { return GetComponent<MaterialSprayer>(); } }

    public Material Material
    {
        get
        {
            if (Unit.Buildable.IsProject)
                return buildable_material_instance;
            else
                return default_material_instance;
        }
    }

    private void Start()
    {
        default_material_instance = new Material(DefaultMaterial);
        buildable_material_instance = new Material(BuildableMaterial);
    }

    private void Update()
    {
        if (Unit.Buildable.IsProject)
        {
            float min_y = Unit.transform.position.y;
            float max_y = min_y + Unit.Physical.Size * 2 * BaseHeight;
            Material.SetFloat(YMaskName, Mathf.Lerp(min_y, max_y, Unit.Buildable.Completion));
        }

        Material.SetVector(ColorName, Unit.Team.Color);

        MaterialSprayer.Material = Material;
    }
}
