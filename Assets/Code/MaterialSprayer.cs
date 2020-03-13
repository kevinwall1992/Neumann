using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteAlways]
public class MaterialSprayer : MonoBehaviour
{
    public Material Material;

    public UnitMeshController UnitMeshController { get { return GetComponent<UnitMeshController>(); } }

    void Start()
    {

    }

    void Update()
    {
        foreach (MeshRenderer mesh in GetComponentsInChildren<MeshRenderer>())
            mesh.material = Material;
    }
}
