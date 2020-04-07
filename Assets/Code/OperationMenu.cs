using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class OperationMenu : Drawer
{
    public List<OperationTile> OperationTiles
    {
        get { return GetComponentsInChildren<OperationTile>().ToList(); }
    }

    List<Operation> abilities;
    public List<Operation> Abilities
    {
        get { return abilities; }

        set
        {
            abilities = value;
            if (abilities == null)
                return;
            
            Reset();

            foreach (Operation operation in abilities)
                Add(OperationTile.Create(operation)).transform.position = SpawnPosition.position;
        }
    }

    public UnitInterface UnitInterface
    {
        get { return GetComponentInParent<UnitInterface>(); }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
    }
}
