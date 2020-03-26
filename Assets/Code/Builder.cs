using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(Unit))]
public class Builder : Able
{
    Stock.Request request = null;

    public NanolathingLineController NanolathingLineController;
    public float Rate = 2;
    public float Reach = 4;
    public List<Buildable> Blueprints = new List<Buildable>();
    public bool CanBuildSelf = false;

    public Buildable Project { get; set; }
    public bool IsProjectWithinReach
    {
        get
        {
            if (Project == null)
                return false;
            
            return IsWithinReach(Project.transform.position);
        }
    }

    public override IEnumerable<Task> Abilities
    {
        get { return Blueprints.Select(project => new BuildTask(project)); }
    }

    protected void Start()
    {
        if (CanBuildSelf && Scene.Main.Prefabs.Units.ContainsKey(Unit.Name))
            Blueprints.Add(Scene.Main.Prefabs.Units[Unit.Name].Buildable);
    }

    void Update()
    {
        if (Unit.Task is BuildTask)
            this.Start<BuildBehavior>().BuildTask = Unit.Task as BuildTask;
        else
            this.Stop<BuildBehavior>();

        if(IsProjectWithinReach)
        {
            if(request == null)
                request = Unit.Team.Stock.MakeRequest(Project.RequiredResources.Normalized() * Rate);

            NanolathingLineController.NanolathingRate = Rate * request.Yield;
            NanolathingLineController.Target = Project.transform.position + new Vector3(0, 0.5f, 0);

            Project.Foundation.PutIn(request.Disbursement);
        }
        else if(request != null)
        {
            request.Revoke();
            request = null;

            NanolathingLineController.NanolathingRate = 0;
        }
    }

    public void Build(Buildable buildable)
    {
        Project = buildable;
    }

    public void StopBuilding()
    {
        Project = null;
    }

    public bool IsWithinReach(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= Reach;
    }


    static Dictionary<string, Buildable> SelfPrefabDictionary = new Dictionary<string, Buildable>();
}


[System.Serializable]
public class BuildTask : Task
{
    public Buildable Blueprint { get; private set; }

    public Vector3 ConstructionSite { get { return Target.Position; } }

    public Buildable Project { get; private set; }
    public override bool IsComplete
    {
        get{ return Project == null ? false : !Project.IsProject; }
    }

    public BuildTask(Buildable blueprint)
    {
        Blueprint = blueprint;
    }

    public Buildable PlaceBlueprint()
    {
        Project = Blueprint.InstantiateProject(ConstructionSite);
        Project.transform.SetParent(Unit.Team.transform);

        return Project;
    }

    public override Operation Instantiate()
    {
        return new BuildTask(Blueprint);
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.BuildTask; } }


    public static BuildTask CreateHelpBuildTask(Buildable project)
    {
        BuildTask help_build_task = new BuildTask(null);
        help_build_task.Project = project;

        return help_build_task;
    }
}