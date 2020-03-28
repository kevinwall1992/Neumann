using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Terrain))]
[RequireComponent(typeof(TerrainCollider))]
public class CelestialBody : MonoBehaviour
{
    public Terrain Terrain { get { return GetComponent<Terrain>(); } }
    public TerrainCollider TerrainCollider { get { return GetComponent<TerrainCollider>(); } }

    public bool IsPointedAt
    {
        get
        {
            RaycastHit hit = new RaycastHit();
            return TerrainCollider.Raycast(Scene.Main.Camera.ScreenPointToRay(Input.mousePosition), out hit, 1000);
        }
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public Vector3 GetSurfaceNormal(Vector3 position)
    {
        RaycastHit hit = new RaycastHit();
        TerrainCollider.Raycast(new Ray(position + new Vector3(0, 100, 0), new Vector3(0, -1, 0)), out hit, 1000);

        return hit.normal;
    }

    public float GetSurfaceHeight(Vector3 position)
    {
        return Terrain.SampleHeight(position);
    }

    public Vector3 GetWorldPositionPointedAt()
    {
        RaycastHit hit = new RaycastHit();
        TerrainCollider.Raycast(Scene.Main.Camera.ScreenPointToRay(Input.mousePosition), out hit, 1000);

        return hit.point;
    }
}
