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

    bool IsDraggable
    {
        get
        {
            return IsInProgramInterface || 
                   transform.parent == Scene.Main.Canvas.transform;
        }
    }

    //Properties seem unnecessary if private****
    [SerializeField]
    Text description_text = null;
    Text DescriptionText { get { return description_text; } }

    [SerializeField]
    Image underlay = null;
    Image Underlay { get { return underlay; } }

    [SerializeField]
    Image discard_overlay = null;
    Image DiscardOverlay { get { return discard_overlay; } }

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

                if (Operation is Task && (Operation as Task) is BuildTask)
                {
                    BuildTask build_task = Operation as BuildTask;
                    Unit unit_blueprint = build_task.Blueprint.GetComponent<Unit>();

                    Underlay.sprite = unit_blueprint.Icon;
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

    public bool IsInOperationMenu { get { return Drawer is OperationMenu; } }
    public bool IsInProgramInterface { get { return Drawer is ProgramInterface; } }

    public Unit Unit
    {
        get
        {
            UnitInterface unit_interface = GetComponentInParent<UnitInterface>();
            if (unit_interface == null)
                return null;

            return unit_interface.Unit;
        }
    }

    protected override void Start()
    {
        base.Start();

        //Have to do this because Tile prefab is super-prefab
        //to OperationTile prefab, and underlay can't come first
        //in the editor because of prefab rules. 
        Underlay.transform.SetAsFirstSibling();

        line.gameObject.SetActive(true);
        line.enabled = false;
    }

    protected override void Update()
    {
        if (Operation == null)
            return;

        if (Unit != null && Operation is Task && (Operation as Task) is BuildTask)
            Underlay.color = Unit.Team.Color;

        DescriptionText.gameObject.SetActive(this.IsPointedAt());
  
        SelectionOverlay.gameObject.SetActive(IsSelected);
        line.enabled = IsSelected;
        if (IsSelected)
        {
            line.SetPosition(0, Scene.Main.Camera.ScreenToWorldPoint(new Vector3(transform.position.x, transform.position.y, 5)));
            line.SetPosition(1, Scene.Main.World.GetWorldPositionPointedAt());

            if (Scene.Main.World.Terrain.gameObject.IsTouched() && this.UseMouseLeftRelease())
            {
                Operation operation = Operation.Instantiate();
                operation.Input.PrimaryVariableName =
                    Scene.Main.World.MemorizePosition(Scene.Main.World.GetWorldPositionPointedAt());

                Unit.Program.Add(operation);
                operation.Execute(Unit);

                IsSelected = false;
            }
        }

        if (!InputUtility.DidDragOccur()  &&
            this.IsPointedAt() && 
            this.UseMouseLeftRelease())
        {
            if (IsInOperationMenu)
                IsSelected = true;
            else if (IsInProgramInterface)
            {
                Unit.Program.Next = Operation;
                Operation.Execute(Unit);
            }
        }

        if (InputUtility.WasMouseRightReleased())
        {
            if (IsSelected)
                IsSelected = false;
        }

        base.Update();
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        base.OnBeginDrag(eventData);

        Drawer.Remove(this);
        transform.SetParent(Scene.Main.Canvas.transform);
    }

    public override void OnDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        base.OnDrag(eventData);

        if ((Scene.Main.UnitInterface.ProgramInterface.transform as RectTransform).ContainsMouse())
        {
            Scene.Main.UnitInterface.ProgramInterface.PreviewAt(Input.mousePosition);
            DiscardOverlay.gameObject.SetActive(false);
        }
        else
        {
            Scene.Main.UnitInterface.ProgramInterface.StopPreviewing();
            DiscardOverlay.gameObject.SetActive(true);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        base.OnEndDrag(eventData);

        DiscardOverlay.gameObject.SetActive(false);

        if ((Scene.Main.UnitInterface.ProgramInterface.transform as RectTransform).ContainsMouse())
            Scene.Main.UnitInterface.ProgramInterface.AddAt(this, Input.mousePosition);
        else
            GameObject.Destroy(gameObject);
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

