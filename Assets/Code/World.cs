using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Physics))]
public class World : MonoBehaviour, HasVariables
{
    [SerializeField]
    Stock stock = null;
    public Stock Stock { get { return stock; } }

    [SerializeField]
    Terrain terrain = null;
    public Terrain Terrain { get { return terrain; } }

    [SerializeField]
    TerrainCollider terrain_collider = null;
    public TerrainCollider TerrainCollider { get { return terrain_collider; } }

    [SerializeField]
    Physics physics = null;
    public Physics Physics { get { return physics; } }

    public bool IsPointedAt
    {
        get
        {
            RaycastHit hit = new RaycastHit();
            return TerrainCollider.Raycast(Scene.Main.Camera.ScreenPointToRay(Input.mousePosition), out hit, 1000);
        }
    }

    public List<Variable> Variables
    {
        get
        {
            return Stock.Variables;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(OperationTile.Selected == null && 
           InputUtility.WasMouseLeftReleased() && 
           TerrainCollider.gameObject.IsTouched())
            Scene.Main.UnitInterface.Unit = null;
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
