using UnityEngine;
using System.Collections;

public class UnitLoadUnloadVisualizationController : MonoBehaviour
{
    [SerializeField]
    Unit unit = null;

    [SerializeField]
    LineRenderer input_line = null;
    [SerializeField]
    CircleLineController input_circle = null;

    [SerializeField]
    LineRenderer output_line = null;
    [SerializeField]
    CircleLineController output_circle = null;

    [SerializeField]
    LineRenderer waste_line = null;
    [SerializeField]
    CircleLineController waste_circle = null;

    [SerializeField]
    float Height = 0.5f;

    UnitInterface UnitInterface { get { return Scene.Main.UnitInterface; } }

    public Unit Unit { get { return unit; } }

    void Start()
    {
        foreach(GameObject game_object in Utility.List(
            input_line.gameObject, input_circle.Line.gameObject, 
            output_line.gameObject, output_circle.Line.gameObject,
            waste_line.gameObject, waste_circle.Line.gameObject))
        {
            game_object.SetActive(true);
            game_object.transform.SetParent(Scene.Main._3DUIElementContainer.transform);
            game_object.transform.localScale = new Vector3(1, 1, 1);
        }
    }

    void Update()
    {
        input_line.enabled = false;
        input_circle.Line.enabled = false;

        output_line.enabled = false;
        output_circle.Line.enabled = false;

        waste_line.enabled = false;
        waste_circle.Line.enabled = false;

        if (!Unit.IsSelected)
            return;

        Vector3 normal_displacment = new Vector3(0, Height, 0);
        OperationTile operation_tile_touched = InputUtility.GetElementTouched<OperationTile>();


        //Load site visualization
        HasLoadSite has_load_site = null;

        if (OperationTileNode.Selected != null && 
            OperationTileNode.Selected.OperationTile.Operation is HasLoadSite)
            has_load_site = OperationTileNode.Selected.OperationTile.Operation as HasLoadSite;

        else if (operation_tile_touched != null &&
                operation_tile_touched.Operation is HasLoadSite &&
                operation_tile_touched.Operation is Task &&
                (operation_tile_touched.Operation as Task).Target != null)
            has_load_site = operation_tile_touched.Operation as HasLoadSite;

        else if (Unit.HasComponent<HasLoadSite>())
            has_load_site = Unit.GetComponent<HasLoadSite>();

        if (has_load_site != null && has_load_site.HasLoadSite)
        {
            Vector3 start_position = has_load_site.LoadSite + normal_displacment;
            Vector3 end_position = Unit.Physical.Position + normal_displacment;
            Vector3 displacement = end_position - start_position;

            if (has_load_site.LoadSite.Distance(Unit.Physical.Position) >= has_load_site.LoadSiteRadius)
            {
                input_line.enabled = true;

                if (displacement.magnitude > (has_load_site.LoadSiteRadius + Unit.Physical.Size * Mathf.Sqrt(2)))
                {
                    input_line.SetPosition(0, start_position + displacement.normalized *
                                                               has_load_site.LoadSiteRadius);
                    input_line.SetPosition(1, end_position - displacement.normalized * Unit.Physical.Size * Mathf.Sqrt(2));
                }
                else
                {
                    input_line.SetPosition(0, start_position + displacement.normalized * has_load_site.LoadSiteRadius);
                    input_line.SetPosition(1, input_line.GetPosition(0) + displacement.normalized * 0.1f);
                }
            }

            input_circle.Line.enabled = true;
            input_circle.transform.position = start_position;
            input_circle.Radius = has_load_site.LoadSiteRadius;
        }


        //Unload site visualization
        HasUnloadSite has_unload_site = null;

        if (OperationTileNode.Selected != null && 
            OperationTileNode.Selected.OperationTile.Operation is HasUnloadSite)
            has_unload_site = OperationTileNode.Selected.OperationTile.Operation as HasUnloadSite;

        else if (operation_tile_touched != null &&
                operation_tile_touched.Operation is HasUnloadSite &&
                operation_tile_touched.Operation is Task &&
                (operation_tile_touched.Operation as Task).Target != null)
            has_unload_site = operation_tile_touched.Operation as HasUnloadSite;

        else if (Unit.HasComponent<HasUnloadSite>())
            has_unload_site = Unit.GetComponent<HasUnloadSite>();

        if (has_unload_site != null && has_unload_site.HasUnloadSite)
        {
            SurfaceDeposit deposit = Scene.Main.World.Asteroid.Surface
                .GetNearestOverlappingDeposit(has_unload_site.UnloadSite) as SurfaceDeposit;

            Vector3 start_position = Unit.Physical.Position + normal_displacment;
            Vector3 end_position = has_unload_site.UnloadSite + normal_displacment;
            Vector3 displacement = end_position - start_position;

            output_line.enabled = true;
            output_line.SetPosition(0, start_position + displacement.normalized * Mathf.Sqrt(2) * Unit.Physical.Size);
            output_line.SetPosition(1, end_position);

            if (deposit != null)
            {
                if (deposit.transform.position.Distance(Unit.Physical.Position) >= deposit.Extent)
                {
                    end_position = deposit.transform.position + normal_displacment;
                    output_line.SetPosition(1, end_position - displacement.normalized * (deposit.Extent + 1));

                    if ((output_line.GetPosition(1) - output_line.GetPosition(0)).Dot(displacement.normalized) < 2)
                        output_line.SetPosition(0, output_line.GetPosition(1) - displacement.normalized * 2);
                }
                else
                    output_line.enabled = false;

                output_circle.Line.enabled = true;
                output_circle.transform.position = deposit.transform.position + normal_displacment;
                output_circle.Radius = deposit.Extent;
            }
        }


        SelectWasteSiteOperation select_waste_site_operation = null;
        Waster waster = null;

        if (OperationTileNode.Selected != null &&
            OperationTileNode.Selected.OperationTile.Operation is SelectWasteSiteOperation)
            select_waste_site_operation = OperationTileNode.Selected.OperationTile
                .Operation as SelectWasteSiteOperation;

        else if (operation_tile_touched != null &&
                operation_tile_touched.Operation is SelectWasteSiteOperation && 
                (operation_tile_touched.Operation as SelectWasteSiteOperation).Input.IsConnected(Unit))
            select_waste_site_operation = (operation_tile_touched.Operation as SelectWasteSiteOperation);

        else if (Unit.HasComponent<Waster>())
        {
            waster = Unit.GetComponent<Waster>();

            if (waster.WasteNot)
                waster = null;
        }

        if (waster != null || select_waste_site_operation != null)
        {
            Vector3 waste_site;
            if (waster != null)
                waste_site = waster.WasteSite;
            else if (select_waste_site_operation.Input.IsConnected(Unit))
                waste_site = select_waste_site_operation.Input.Read<Vector3>(Unit);
            else
                waste_site = Scene.Main.World.Asteroid.GetWorldPositionPointedAt();

            SurfaceDeposit deposit = Scene.Main.World.Asteroid.Surface
                .GetNearestOverlappingDeposit(waste_site) as SurfaceDeposit;

            Vector3 start_position = Unit.Physical.Position + normal_displacment;
            Vector3 end_position = waste_site + normal_displacment;
            Vector3 displacement = end_position - start_position;

            waste_line.enabled = true;
            waste_line.SetPosition(0, start_position + displacement.normalized * Mathf.Sqrt(2) * Unit.Physical.Size);
            waste_line.SetPosition(1, end_position);

            if (deposit != null)
            {
                if (deposit.transform.position.Distance(Unit.Physical.Position) >= deposit.Extent)
                {
                    end_position = deposit.transform.position + normal_displacment;
                    waste_line.SetPosition(1, end_position - displacement.normalized * (deposit.Extent + 1));

                    if ((waste_line.GetPosition(1) - waste_line.GetPosition(0)).Dot(displacement.normalized) < 2)
                        waste_line.SetPosition(0, waste_line.GetPosition(1) - displacement.normalized * 2);
                }
                else
                    waste_line.enabled = false;

                waste_circle.Line.enabled = true;
                waste_circle.transform.position = deposit.transform.position + normal_displacment;
                waste_circle.Radius = deposit.Extent;
            }
        }
    }
}
