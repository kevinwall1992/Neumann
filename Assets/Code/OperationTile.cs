using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OperationTile : Tile
{
    [SerializeField]
    Sprite null_task_sprite = null;

    [SerializeField]
    LineRenderer line = null;

    [SerializeField]
    Text description_text = null;
    Text DescriptionText { get { return description_text; } }

    [SerializeField]
    Image underlay = null;
    Image Underlay { get { return underlay; } }

    [SerializeField]
    Operation operation;
    public Operation Operation
    {
        get { return operation; }

        set
        {
            operation = value;

            Underlay.color = Color.clear;

            if (Operation != null)
            {
                Image.sprite = Operation.Style.Sprite;
                Image.color = Operation.Style.Color;
                DescriptionText.text = Operation.Style.Description;

                if(Operation is Task && (Operation as Task) is BuildTask)
                {
                    BuildTask build_task = Operation as BuildTask;
                    Unit unit = build_task.Blueprint.GetComponent<Unit>();

                    Underlay.sprite = unit.Icon;
                    Underlay.color = unit.Color;
                }
            }
            else
            {
                Image.sprite = null_task_sprite;
                Image.color = Color.white;
                DescriptionText.text = "";
            }
        }
    }

    public bool IsSelected
    {
        get { return Selected == this; }
        set
        {
            if (value)
                Selected = this;
            else if (IsSelected)
                Selected = null;
        }
    }

    public Unit Unit { get { return GetComponentInParent<UnitInterface>().Unit; } }

    protected override void Start()
    {
        base.Start();

        //Have to do this because Tile prefab is super-prefab
        //to OperationTile prefab, and underlay can't come first
        //in the editor because of prefab rules. 
        Underlay.transform.SetAsFirstSibling();
    }

    protected override void Update()
    {
        base.Update();

        DescriptionText.gameObject.SetActive(this.IsPointedAt());

        SelectionOverlay.gameObject.SetActive(IsSelected);
        line.gameObject.SetActive(IsSelected);
        if (IsSelected)
        {
            line.SetPosition(0, Scene.Main.Camera.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, 5)));
            line.SetPosition(1, Scene.Main.World.GetWorldPositionPointedAt());
        }

        if (InputUtility.WasMouseLeftReleased() || InputUtility.WasMouseRightReleased())
        {
            if (IsSelected && Scene.Main.World.Terrain.gameObject.IsTouched())
            {
                if (Operation is Task)
                {
                    Task task = Operation as Task;
                    Vector3 position = Scene.Main.World.GetWorldPositionPointedAt();

                    if (task is MoveTask)
                        task = (task as MoveTask).Reparameterized(Unit.GetComponent<Physical>(), position);
                    if (task is BuildTask)
                        task = (task as BuildTask).Reparameterized(position);

                    Unit.Task = task;
                }

                IsSelected = false;
            }

            if (this.IsPointedAt())
                IsSelected = true;
        }
    }


    public static OperationTile Selected { get; protected set; }

    public static OperationTile Create(Operation operation)
    {
        OperationTile operation_tile = GameObject.Instantiate(Scene.Main.Prefabs.OperationTile);
        operation_tile.Operation = operation;
        operation_tile.name = operation.ToString() + " OperationTile";
        

        return operation_tile;
    }

    public interface Stylish
    {
        Style.Operation Style { get; }
    }
}

