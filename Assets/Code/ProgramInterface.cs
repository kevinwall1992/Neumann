using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class ProgramInterface : Drawer
{
    public bool IsClosed { get; set; }

    Program program;
    public Program Program
    {
        get { return program; }

        set
        {
            program = value;

            Reset();
        }
    }

    public List<OperationTile> OperationTiles
    {
        get { return GetComponentsInChildren<OperationTile>().ToList(); }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Program == null)
            return;

        IEnumerable<Operation> operations = OperationTiles.Select(operation_tile => operation_tile.Operation);

        foreach(Operation operation in Program)
            if (!operations.Contains(operation))
                Add(OperationTile.Create(operation));

        foreach(OperationTile operation_tile in OperationTiles)
            if(!Program.Contains(operation_tile.Operation))
            {
                Remove(operation_tile);
                GameObject.Destroy(operation_tile.gameObject);
            }

    }

    public override void Reset()
    {
        base.Reset();

        RowsVisible = 0;
    }
}
