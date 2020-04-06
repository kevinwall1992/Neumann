using UnityEngine;
using System.Collections;


[RequireComponent(typeof(Builder))]
public class BuildBehavior : Behavior
{
    public BuildTask BuildTask { get; set; }

    public Builder Builder { get { return GetComponent<Builder>(); } }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

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
                BuildTask.PlaceBlueprint();

            Builder.Build(BuildTask.Project);
        }
    }

    internal override void CleanUp()
    {
        base.CleanUp();

        Builder.StopBuilding();
        this.Stop<SeekBehavior>();
    }
}
