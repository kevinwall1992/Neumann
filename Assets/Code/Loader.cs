using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Motile))]
public class Loader : Profession, HasLoadSite, HasUnloadSite
{
    public float Volume;
    public Pile Hold;
    public float DistanceTolerance;
    public float LoadRate;

    public override IEnumerable<Operation> Abilities {
        get { return Utility.List<Operation>(new LoadTask(), new UnloadTask()); }
    }

    public bool IsLoaded { get { return Hold.Volume == Volume; } }
    public bool IsUnloaded { get { return Hold.Volume == 0; } }

    public bool HasLoadSite { get { return Unit.Task is LoadTask; } }
    public Vector3 LoadSite { get { return (Unit.Task as LoadTask).Target.Position; } }
    public float LoadSiteRadius { get { return GetRange(Volume); } }

    public bool HasUnloadSite { get { return Unit.Task is UnloadTask; } }
    public Vector3 UnloadSite { get { return (Unit.Task as UnloadTask).Target.Position; } }

    void Start()
    {

    }

    void Update()
    {
        if (Unit.Task is LoadTask)
        {
            LoadTask load_task = Unit.Task as LoadTask;

            if (!load_task.IsAtSource)
                this.Start<SeekBehavior>().Target = load_task.Target;
            else
            {
                this.Stop<SeekBehavior>();
                Load();
            }
        }
        else if (Unit.Task is UnloadTask)
        {
            UnloadTask unload_task = Unit.Task as UnloadTask;



            if (!unload_task.IsAtDestination)
                this.Start<SeekBehavior>().Target = unload_task.Target;
            else
            {
                this.Stop<SeekBehavior>();
                Unload();
            }
        }
        else
            this.Stop<SeekBehavior>();
    }

    public void Load()
    {
        float sample_volume = Mathf.Min((LoadRate * Time.deltaTime), Volume - Hold.Volume);

        Hold.PutIn(Scene.Main.World.Asteroid.Regolith
            .TakeSample(Unit.Physical.Position, GetRange(Volume), sample_volume));
    }

    public void Unload()
    {
        float dump_volume = Mathf.Min((LoadRate * Time.deltaTime), Hold.Volume);

        Scene.Main.World.Asteroid.Litter(Unit.Physical.Position, Hold.TakeOut(dump_volume));
    }


    public static float GetRange(float volume)
    {
        return Mathf.Sqrt(volume);
    }
}


public class LoadTask : Task, HasLoadSite
{
    public override bool IsComplete { get { return Loader.IsLoaded; } }

    public bool IsAtSource
    {
        get
        {
            return Unit.Physical.Position
                .Distance(Target.Position) < Loader.DistanceTolerance;
        }
    }

    public bool HasLoadSite { get { return true; } }
    public Vector3 LoadSite { get { return SpeculativeTarget.Position; } }

    public float LoadSiteRadius
    {
        get
        {
            Loader loader = Scene.Main.UnitInterface.Unit.GetComponent<Loader>();

            return Loader.GetRange(loader.Volume);
        }
    }

    public Loader Loader { get { return Unit.GetComponent<Loader>(); } }

    public LoadTask()
        
    {
        
    }

    public override Operation Instantiate()
    {
        return new LoadTask();
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.LoadTask; } }
}


public class UnloadTask : Task, HasUnloadSite
{
    public override bool IsComplete { get { return Loader.IsUnloaded; } }

    public bool IsAtDestination
    {
        get
        {
            return Unit.Physical.Position
                .Distance(Target.Position) < Loader.DistanceTolerance;
        }
    }

    public bool HasUnloadSite { get { return true; } }
    public Vector3 UnloadSite { get { return SpeculativeTarget.Position; } }

    public Loader Loader { get { return Unit.GetComponent<Loader>(); } }

    public UnloadTask()

    {

    }

    public override Operation Instantiate()
    {
        return new UnloadTask();
    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.UnloadTask; } }
}


public interface HasLoadSite
{
    bool HasLoadSite { get; }
    Vector3 LoadSite { get; }
    float LoadSiteRadius { get; }
}

public interface HasUnloadSite
{
    bool HasUnloadSite { get; }
    Vector3 UnloadSite { get; }
}
