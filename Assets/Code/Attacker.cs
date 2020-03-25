﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attacker : Able
{
    public override IEnumerable<Task> Abilities
    {
        get { return Utility.List<Task>(new AttackTask()); }
    }

    void Update()
    {
        if (Task is AttackTask)
            this.Start<SeekBehavior>().Target = (Task as AttackTask).Target;
    }
}

public class Gunner : Attacker
{

}

public class MobileLauncher : Attacker
{

}

public class SuicideBomber : Attacker
{

}


[System.Serializable]
public class AttackTask : Task
{
    public override bool IsComplete
    {
        get
        {
            if (Target.IsMortal)
                return Target.Mortal.IsDead;

            return false;
        }
    }

    public override Operation Instantiate()
    {
        return new AttackTask();
    }

    public override Style.Operation Style
    { get { return Scene.Main.Style.AttackTask; } }
}
