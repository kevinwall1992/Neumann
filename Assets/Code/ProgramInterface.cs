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

    public Unit Unit { get { return GetComponentInParent<UnitInterface>().Unit; } }

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
                base.Add(OperationTile.Create(operation)).transform.position = SpawnPosition.position;

        foreach(OperationTile operation_tile in OperationTiles)
            if(!Program.Contains(operation_tile.Operation))
            {
                base.Remove(operation_tile);
                GameObject.Destroy(operation_tile.gameObject);
            }


        foreach(Operation operation in Program)
            OperationTiles.Find(operation_tile => operation_tile.Operation == operation)
                .transform.SetAsFirstSibling();

        if (Unit.Program.Next != null)
        {
            OperationTile operation_tile = OperationTiles.Find(operation_tile_ => 
                operation_tile_.Operation == Unit.Program.Next);
            if (operation_tile.IsBeingDragged)
                return;
        }
    }

    public override Tile Add(Tile tile)
    {
        base.Add(tile);

        Operation operation = (tile as OperationTile).Operation;

        if (!program.Contains(operation))
            Program.Add(operation);

        return tile;
    }

    public override Tile Remove(Tile tile)
    {
        Operation operation = (tile as OperationTile).Operation;

        if (program.Contains(operation))
            Program.Remove(operation);

        return tile;
    }

    public override Tile AddAt(Tile tile, Vector2 position)
    {
        base.Add(tile);

        Operation operation = (tile as OperationTile).Operation;

        int insertion_index = Program.Count - PositionToInsertionIndex(position);

        if (Program.Contains(operation))
        {
            Program.Remove(operation);
            insertion_index--;
        }
        
        Program.Insert(insertion_index, operation);

        return tile;
    }
}
