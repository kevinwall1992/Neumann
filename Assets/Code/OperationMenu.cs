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

    List<Task> abilities;
    public List<Task> Abilities
    {
        get { return abilities; }

        set
        {
            abilities = value;
            if (abilities == null)
                return;
            
            Reset();

            List<Operation> operations =
            Utility.List<Operation>(new ChooseOperation(),
                                    new WriteOperation(), 
                                    new InterruptOperation(),
                                    new YieldOperation());
            operations.AddRange(Abilities);

            if (UnitInterface.Unit.HasComponent<Waster>())
                operations.Add(new SelectWasteSiteOperation());

            foreach (Operation operation in operations)
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
