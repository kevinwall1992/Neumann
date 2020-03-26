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
            if (value != null)
                abilities = value;
            else
                abilities = new List<Task>();

            Reset();

            List<Operation> operations =
            Utility.List<Operation>(new ChooseOperation(),
                                    new YieldOperation());
            operations.AddRange(Abilities);

            foreach (Operation operation in operations)
                Add(OperationTile.Create(operation)).transform.position = SpawnPosition.position;
        }
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
