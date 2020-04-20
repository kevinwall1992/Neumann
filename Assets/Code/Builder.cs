using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


[RequireComponent(typeof(Unit))]
public class Builder : Profession
{
    Stock.Request request = null;

    public NanolathingLineController NanolathingLineController;
    public float Rate = 2;
    public float Reach = 4;
    public List<Buildable> Blueprints = new List<Buildable>();
    public bool CanBuildSelf = false;
    public Vector3 FixedConstructionSite;
    public bool UseFixedConstructionSite;

    public Buildable Project { get; set; }
    public bool IsProjectWithinReach
    {
        get
        {
            if (Project == null)
                return false;
            
            return IsWithinReach(Project.transform.position, Project.transform.localScale.x);
        }
    }

    public override IEnumerable<Operation> Abilities
    {
        get
        {
            return Blueprints.Select(project => UseFixedConstructionSite ? 
                new BuildTask(project, FixedConstructionSite) : 
                new BuildTask(project));
        }
    }

    protected void Start()
    {
        if (CanBuildSelf && Scene.Main.Prefabs.Units.ContainsKey(Unit.Name))
            Blueprints.Add(Scene.Main.Prefabs.Units[Unit.Name].Buildable);

        NanolathingLineController.Line.gameObject.SetActive(true);
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

    public bool IsWithinReach(Vector3 position, float size)
    {
        return (Vector3.Distance(transform.position, position) - (size + Unit.Physical.Size)) <= Reach;
    }


    static Dictionary<string, Buildable> SelfPrefabDictionary = new Dictionary<string, Buildable>();
}


[System.Serializable]
public class BuildTask : Task
{
    Vector3 fixed_construction_site;
    bool has_fixed_construction_site;

    public Buildable Blueprint { get; private set; }

    public Vector3 ConstructionSite
    {
        get
        {
            if (has_fixed_construction_site)
                return Unit.transform.TransformPoint(fixed_construction_site);
            else
                return Target.Position;
        }
    }
    public float ConstructionSiteSize { get { return Blueprint.transform.localScale.x; } }

    public bool IsConstructionSiteClear
    {
        get
        {
            IEnumerable<Physical> obstructors = 
                Scene.Main.World.GetComponentsInChildren<Physical>()
                    .Where(physical => physical.Position.Distance(ConstructionSite) < 
                                       (ConstructionSiteSize + physical.Size));

            foreach (Physical physical in obstructors)
                if ((Project != null && physical.gameObject != Project.gameObject) || 
                    Blueprint.HasComponent<Motile>() == physical.HasComponent<Motile>())
                    return false;

            return true;
        }
    }

    public Buildable Project { get; private set; }

    public override bool IsComplete
    {
        get{ return Project == null ? false : !Project.IsProject; }
    }

    public override bool TakesInput
    {
        get { return !has_fixed_construction_site; }
    }

    public BuildTask(Buildable blueprint)
    {
        Blueprint = blueprint;
    }

    public BuildTask(Buildable blueprint, Vector3 construction_site)
    {
        Blueprint = blueprint;

        fixed_construction_site = construction_site;
        has_fixed_construction_site = true;
    }

    public Buildable PlaceBlueprint()
    {
        Project = Blueprint.InstantiateProject(ConstructionSite);
        Project.transform.SetParent(Unit.Team.transform);

        return Project;
    }

    public override Operation Instantiate()
    {
        if (has_fixed_construction_site)
            return new BuildTask(Blueprint, fixed_construction_site);
        else
            return new BuildTask(Blueprint);
    }


    public override Style.Operation Style
    {
        get
        {
            if (!Blueprint.HasComponent<Unit>())
                return Scene.Main.Style.BuildTask;

            Unit unit_blueprint = Blueprint.GetComponent<Unit>();

            return Scene.Main.Style.BuildTask
                .WithUnderlay(unit_blueprint.Icon, Scene.Main.UnitInterface.Unit.Team.Color)
                .WithDescriptionAmended(unit_blueprint.Name);
        }
    }


    public static BuildTask CreateHelpBuildTask(Buildable project)
    {
        BuildTask help_build_task = new BuildTask(null);
        help_build_task.Project = project;

        return help_build_task;
    }
}