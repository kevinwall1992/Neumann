using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Graph;

[RequireComponent(typeof(Builder))]
public class BuildBehavior : Behavior
{
    public BuildTask BuildTask { get; set; }

    public Builder Builder { get { return GetComponent<Builder>(); } }

    protected override void Update()
    {
        base.Update();

        BuildTask.ValidateConstructionSite(Builder);

        if (BuildTask.Target == null)
            return;

        if (!Builder.IsWithinReach(BuildTask.ConstructionSite, BuildTask.ConstructionSiteSize))
        {
            Vector3 edge_of_construction_site =
                BuildTask.ConstructionSite +
                (transform.position - BuildTask.ConstructionSite).normalized * Builder.Reach * 0.9f;

            this.Start<SeekBehavior>().Target = edge_of_construction_site;

            Builder.StopBuilding();
        }
        else
        {
            this.Stop<SeekBehavior>();

            if (BuildTask.Project == null)
            {
                if (BuildTask.IsConstructionSiteClear)
                    BuildTask.PlaceBlueprint();
                else
                    return;
            }

            Builder.Build(BuildTask.Project);
        }
    }

    protected override void OnDestroy()
    {
        Builder.StopBuilding();

        base.OnDestroy();
    }

    internal override void CleanUp()
    {
        base.CleanUp();

        Builder.StopBuilding();
        this.Stop<SeekBehavior>();
    }
}
