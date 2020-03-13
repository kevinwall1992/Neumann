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

            Reset();

            List<Operation> operations =
            Utility.List<Operation>(new AgainOperation(),
                                          new ChooseOperation(),
                                          new YieldOperation());
            operations.AddRange(Abilities);

            foreach (Operation operation in operations)
                Add(OperationTile.Create(operation));
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
