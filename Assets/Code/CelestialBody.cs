using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Terrain))]
[RequireComponent(typeof(TerrainCollider))]
public class CelestialBody : MonoBehaviour
{
    IEnumerable<SurfaceDeposit> SurfaceDeposits { get { return Surface.GetComponentsInChildren<SurfaceDeposit>(); } }

    public Terrain Terrain { get { return GetComponent<Terrain>(); } }
    public TerrainCollider TerrainCollider { get { return GetComponent<TerrainCollider>(); } }

    [SerializeField]
    Stratum surface = null;
    public Stratum Surface { get { return surface; } }

    public bool IsPointedAt
    {
        get
        {
            RaycastHit hit = new RaycastHit();
            return TerrainCollider.Raycast(Scene.Main.Camera.ScreenPointToRay(Input.mousePosition), out hit, 1000);
        }
    }

    protected virtual void Start()
    {

    }

    protected virtual void Update()
    {
        List<SurfaceDeposit> surface_deposits = SurfaceDeposits.OrderBy(surface_deposit => surface_deposit.Extent).Reverse().ToList();

        while(surface_deposits.Count > 0)
        {
            SurfaceDeposit surface_deposit = surface_deposits.TakeAt(0);

            foreach (SurfaceDeposit other_surface_deposit in new List<SurfaceDeposit>(SurfaceDeposits))
            {
                if (other_surface_deposit == surface_deposit)
                    continue;

                float distance = surface_deposit.transform.position
                    .Distance(other_surface_deposit.transform.position);

                if (distance <= surface_deposit.Extent)
                {
                    surface_deposit.AddTo(other_surface_deposit.Composition.Normalized() * other_surface_deposit.Volume);

                    surface_deposits.Remove(other_surface_deposit);
                    other_surface_deposit.transform.SetParent(null);
                    GameObject.Destroy(other_surface_deposit.gameObject);
                }

            }
        } 
    }

    public void Litter(Vector3 position, Pile pile)
    {
        if (pile.Volume == 0)
            return;

        SurfaceDeposit nearest_deposit = SurfaceDeposits.ToList()
            .Find(deposit => deposit.transform.position.Distance(position) <= deposit.Extent);

        if (nearest_deposit == null)
        {
            nearest_deposit = SurfaceDeposit.Create();
            nearest_deposit.transform.SetParent(Surface.transform);
            nearest_deposit.transform.position = position;
        }


        nearest_deposit.AddTo(pile);
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

    //In W/m^2
    public float GetSolarFlux(Vector3 position)
    {
        return 1300;
    }
}
