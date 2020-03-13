using UnityEngine;
using System.Collections;

public class FloatMaterialPropertyController : MonoBehaviour
{
    [SerializeField]
    public FloatMaterialProperty material_property;

    public float Value;

    void Start()
    {

    }

    void Update()
    {
        material_property.Value = Value;
    }
}
