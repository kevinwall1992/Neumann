﻿using UnityEngine;
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
    float Height = 0.5f;

    UnitInterface UnitInterface { get { return Scene.Main.UnitInterface; } }

    public Unit Unit { get { return unit; } }

    void Start()
    {
        Transform container = Scene.Main._3DUIElementContainer.transform;

        input_line.gameObject.SetActive(true);
        input_line.transform.SetParent(container);

        input_circle.Line.gameObject.SetActive(true);
        input_circle.Line.transform.SetParent(container);

        output_line.gameObject.SetActive(true);
        output_line.transform.SetParent(container);

        output_circle.Line.gameObject.SetActive(true);
        output_circle.Line.transform.SetParent(container);
    }

    void Update()
    {
        input_line.enabled = false;
        input_circle.Line.enabled = false;

        output_line.enabled = false;
        output_circle.Line.enabled = false;

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
            Vector3 start_position = Unit.Physical.Position + normal_displacment;
            Vector3 end_position = has_load_site.LoadSite + normal_displacment;
            Vector3 displacement = end_position - start_position;
            float line_length = displacement.magnitude - has_load_site.LoadSiteRadius;

            if (line_length > 0)
            {
                input_line.enabled = true;
                input_line.SetPosition(0, start_position);
                input_line.SetPosition(1, start_position + displacement.normalized * line_length);
            }

            input_circle.Line.enabled = true;
            input_circle.transform.position = end_position;
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

            output_line.enabled = true;
            output_line.SetPosition(0, Unit.Physical.Position + normal_displacment);
            output_line.SetPosition(1, has_unload_site.UnloadSite + normal_displacment);

            if (deposit != null)
            {
                output_circle.Line.enabled = true;
                output_circle.transform.position = deposit.transform.position + normal_displacment;
                output_circle.Radius = deposit.Extent;
            }
        }
    }
}
