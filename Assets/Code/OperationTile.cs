using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OperationTile : Tile
{
    [SerializeField]
    Sprite null_task_sprite = null;

    [SerializeField]
    OperationTileIONode input_node = null, output_node = null;

    [SerializeField]
    OperationTileGotoNode goto_node = null;

    [SerializeField]
    RectTransform goto_attach_position = null;
    public RectTransform GotoAttachPosition { get { return goto_attach_position; } }

    bool IsDraggable
    {
        get
        {
            return IsInProgramInterface || 
                   transform.parent == Scene.Main.Canvas.transform;
        }
    }

    [SerializeField]
    Text description_text = null;

    [SerializeField]
    Image underlay = null;

    [SerializeField]
    Image discard_overlay = null;

    [SerializeField]
    Operation operation;
    public Operation Operation
    {
        get { return operation; }

        set
        {
            operation = value;

            underlay.color = Color.clear;

            input_node.gameObject.SetActive(false);
            output_node.gameObject.SetActive(false);
            goto_node.gameObject.SetActive(false);

            if (Operation != null)
            {
                Image.sprite = Operation.Style.Sprite;
                Image.color = Operation.Style.Color;
                description_text.text = Operation.Style.Description;

                if (Operation.TakesInput)
                    input_node.VariableTile = VariableTile.Find(Operation.Input.PrimaryVariableName);
                if (Operation.HasOutput)
                    output_node.VariableTile = VariableTile.Find(Operation.Output.PrimaryVariableName);
                if (Operation.TakesGoto)
                    goto_node.GotoOperationTile = 
                        Scene.Main.UnitInterface.ProgramInterface.Tiles
                        .Find(tile => (tile as OperationTile).Operation == Operation.Goto) as OperationTile;

                if (Operation is Task && (Operation as Task) is BuildTask)
                {
                    BuildTask build_task = Operation as BuildTask;
                    Unit unit_blueprint = build_task.Blueprint.GetComponent<Unit>();

                    underlay.sprite = unit_blueprint.Icon;
                }
            }
            else
            {
                Image.sprite = null_task_sprite;
                Image.color = Color.white;
                description_text.text = "";
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
        underlay.transform.SetAsFirstSibling();

        input_node.gameObject.SetActive(false);
        output_node.gameObject.SetActive(false);
        goto_node.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (Operation == null)
            return;

        if (Unit != null && Operation is Task && (Operation as Task) is BuildTask)
            underlay.color = Unit.Team.Color;

        description_text.gameObject.SetActive(this.IsPointedAt());
  
        SelectionOverlay.gameObject.SetActive(IsSelected);

        input_node.gameObject.SetActive(Operation.TakesInput);
        output_node.gameObject.SetActive(Operation.HasOutput);
        goto_node.gameObject.SetActive(Operation.TakesGoto);

        if (Operation is BuildTask)
            Image.color = Image.color.AlphaChangedTo(this.IsPointedAt() ? 1 : 0.0f);


        if (IsInProgramInterface)
        {
            if (input_node.VariableTile != null)
                Operation.Input.PrimaryVariableName = input_node.VariableTile.Variable.Name;
            if (output_node.VariableTile != null)
                Operation.Output.PrimaryVariableName = output_node.VariableTile.Variable.Name;
            if (goto_node.GotoOperationTile != null)
                Operation.Goto = goto_node.GotoOperationTile.Operation;
        }
        else if(IsInOperationMenu && IsSelected)
        {
            bool is_complete = false;
            if (operation.TakesGoto)
                is_complete = goto_node.GotoOperationTile != null;
            else if((!operation.TakesInput || input_node.VariableTile != null) && 
                    (!operation.HasOutput || output_node.VariableTile != null))
                is_complete = true;

            if (is_complete)
            {
                Operation operation = Operation.Instantiate();
                if(operation.TakesInput && input_node.VariableTile != null)
                    operation.Input.PrimaryVariableName = input_node.VariableTile.Variable.Name;
                if(operation.HasOutput && output_node.VariableTile != null)
                    operation.Output.PrimaryVariableName = output_node.VariableTile.Variable.Name;
                if (operation.TakesGoto && goto_node.GotoOperationTile != null)
                    operation.Goto = goto_node.GotoOperationTile.Operation;

                Unit.Program.Add(operation);
                if (Unit.Program.Next == null)
                    Unit.Program.Next = operation;
                if (operation is Task && (Unit.Task == null || !Input.GetKey(KeyCode.LeftShift)))
                {
                    Unit.Task = null;
                    operation.Execute(Unit);
                }

                input_node.VariableTile = null;
                input_node.IsSelected = false;
                output_node.VariableTile = null;
                output_node.IsSelected = false;
                goto_node.GotoOperationTile = null;
                goto_node.IsSelected = false;
                IsSelected = false;
            }
        }

        if (!InputUtility.DidDragOccur()  &&
            this.IsPointedAt() && 
            !input_node.IsPointedAt() && !output_node.IsPointedAt() && !goto_node.IsPointedAt() && 
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

        if(IsSelected)
        {
            if (Operation.TakesInput && input_node.VariableTile == null && !Operation.TakesGoto)
            {
                if(!input_node.IsSelected)
                    input_node.IsSelected = true;
            }
            else if (Operation.HasOutput && output_node.VariableTile == null)
            {
                if(!output_node.IsSelected)
                    output_node.IsSelected = true;
            }
            else if (Operation.TakesGoto && goto_node.GotoOperationTile == null)
            {
                if (!goto_node.IsSelected)
                    goto_node.IsSelected = true;
            }
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
            discard_overlay.gameObject.SetActive(false);
        }
        else
        {
            Scene.Main.UnitInterface.ProgramInterface.StopPreviewing();
            discard_overlay.gameObject.SetActive(true);
        }
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDraggable)
            return;

        base.OnEndDrag(eventData);

        discard_overlay.gameObject.SetActive(false);

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

