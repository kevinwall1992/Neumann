using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

public class ProgramInterface : Drawer
{
    [SerializeField]
    Image arrow = null;

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

        arrow.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        base.Update();

        if (Program == null)
            return;

        List<OperationTile> operation_tiles = OperationTiles;
        IEnumerable<Operation> operations = operation_tiles.Select(operation_tile => operation_tile.Operation);

        foreach(Operation operation in Program)
            if (!operations.Contains(operation))
                base.Add(OperationTile.Create(operation)).transform.position = SpawnPosition.position;

        foreach(OperationTile operation_tile in operation_tiles)
            if(!Program.Contains(operation_tile.Operation))
            {
                base.Remove(operation_tile);
                GameObject.Destroy(operation_tile.gameObject);
            }

        operation_tiles = OperationTiles;
        foreach (Operation operation in Program)
            operation_tiles.Find(operation_tile => operation_tile.Operation == operation)
                .transform.SetAsFirstSibling();

        if (Unit.Program.Next != null)
        {
            OperationTile operation_tile = operation_tiles.Find(operation_tile_ => 
                operation_tile_.Operation == Unit.Program.Next);
            if (operation_tile.IsBeingDragged)
                return;

            Vector3 target_position = arrow.transform.position
                .YChangedTo(operation_tile.transform.position.y);

            if (arrow.gameObject.activeSelf)
                arrow.transform.position = Vector3.Lerp(arrow.transform.position,
                                                        target_position,
                                                        4 * Time.deltaTime);
            else
                arrow.transform.position = target_position;

            arrow.gameObject.SetActive(true);
        }
        else
            arrow.gameObject.SetActive(false);
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
