using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Attacker : Profession
{
    public override IEnumerable<Operation> Abilities
    {
        get { return Utility.List(new AttackTask()); }
    }

    public bool IsAttacking { get; private set; }

    protected virtual void Update()
    {
        
    }

    public void Attack() { IsAttacking = true; }
    public void StopAttacking() { IsAttacking = false; }
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
