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

        this.Start<SeekBehavior>().Target = BuildTask.ConstructionSite;

        if (!Builder.IsWithinReach(BuildTask.ConstructionSite))
        {
            Builder.StopBuilding();
        }
        else
        {
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
