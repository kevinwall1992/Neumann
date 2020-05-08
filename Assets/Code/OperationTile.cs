using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class OperationTile : Tile
{
    [SerializeField]
    Sprite null_task_sprite = null;

    [SerializeField]
    OperationTileIONode input_node = null, output_node = null;
    public OperationTileIONode InputNode { get { return input_node; } }
    public OperationTileIONode OutputNode { get { return output_node; } }

    [SerializeField]
    OperationTileGotoNode goto_node = null;
    public OperationTileGotoNode GotoNode { get { return goto_node; } }

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
            underlay.sprite = null;

            InputNode.gameObject.SetActive(false);
            OutputNode.gameObject.SetActive(false);
            GotoNode.gameObject.SetActive(false);

            if (Operation != null)
            {
                Image.sprite = Operation.Style.Sprite;
                Image.color = Operation.Style.Color;
                description_text.text = Operation.Style.Description;

                if (Operation.Style.Underlay != null)
                {
                    underlay.sprite = operation.Style.Underlay;
                    underlay.color = operation.Style.UnderlayColor;
                    Image.color = Image.color.AlphaChangedTo(0);
                }

                if (Operation.TakesInput)
                    InputNode.VariableName = Operation.Input.PrimaryVariableName;
                if (Operation.HasOutput)
                    OutputNode.VariableName = Operation.Output.PrimaryVariableName;
                if (Operation.TakesGoto)
                    GotoNode.GotoOperationTile =
                        Scene.Main.UnitInterface.ProgramInterface.Tiles
                        .Find(tile => (tile as OperationTile).Operation == Operation.Goto) as OperationTile;
            }
            else
            {
                Image.sprite = null_task_sprite;
                Image.color = Color.white;
                description_text.text = "";
            }
        }
    }

    public bool IsSelectable = true;
    public bool IsSelected
    {
        get { return Selected == this; }
        set
        {
            if (value && IsSelectable)
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

    public bool IsOpen
    {
        get
        {
            if (!IsInProgramInterface)
                return false;

            ProgramInterface program_interface = Scene.Main.UnitInterface.ProgramInterface;
            if (!program_interface.IsOpen)
                return false;

            return program_interface.Mask.Contains(transform.position);
        }
    }

    public float MouseDistance
    {
        get
        {
            List<float> distances = Utility.List(transform.position.Distance(Input.mousePosition));

            if (Operation.TakesInput)
            {
                distances.Add(InputNode.MouseDistance);

                if(InputNode.PipeFunctionSlot.PipeFunctionTile != null)
                    distances.Add(InputNode.PipeFunctionSlot.SecondaryInputNode.MouseDistance);
            }
            if (Operation.HasOutput)
            {
                distances.Add(OutputNode.MouseDistance);

                if (OutputNode.PipeFunctionSlot.PipeFunctionTile != null)
                    distances.Add(OutputNode.PipeFunctionSlot.SecondaryInputNode.MouseDistance);
            }

            return distances.Min();
        }
    }

    public bool IsHighlighted { get; set; }

    protected override void Start()
    {
        base.Start();

        //Have to do this because Tile prefab is super-prefab
        //to OperationTile prefab, and underlay can't come first
        //in the editor because of prefab rules. 
        underlay.transform.SetAsFirstSibling();

        InputNode.gameObject.SetActive(false);
        OutputNode.gameObject.SetActive(false);
        GotoNode.gameObject.SetActive(false);
    }

    protected override void Update()
    {
        if (Unit == null || Operation == null)
            return;

        if (underlay.sprite != null)
        {
            float target_image_alpha = 0;
            float target_underlay_alpha = 1;

            if (this.IsPointedAt())
            {
                target_image_alpha = 1;
                target_underlay_alpha = 0.1f;
            }

            Image.color = Image.color.AlphaChangedTo(
                Mathf.Lerp(Image.color.a, target_image_alpha, Time.deltaTime * 5));
            underlay.color = underlay.color.AlphaChangedTo(
                Mathf.Lerp(underlay.color.a, target_underlay_alpha, Time.deltaTime * 5));
        }

        description_text.gameObject.SetActive(this.IsPointedAt());
  
        SelectionOverlay.gameObject.SetActive(IsSelected);

        InputNode.gameObject.SetActive(Operation.TakesInput);
        OutputNode.gameObject.SetActive(Operation.HasOutput);
        GotoNode.gameObject.SetActive(Operation.TakesGoto);


        if (IsInProgramInterface)
        {
            if (InputNode.VariableTile != null)
                Operation.Input.PrimaryVariableName = InputNode.VariableTile.Variable.Name;
            if (OutputNode.VariableTile != null)
                Operation.Output.PrimaryVariableName = OutputNode.VariableTile.Variable.Name;
            if (GotoNode.GotoOperationTile != null)
                Operation.Goto = GotoNode.GotoOperationTile.Operation;
        }
        else if(IsInOperationMenu && IsSelected)
        {
            bool is_complete = false;
            if (operation.TakesGoto)
                is_complete = GotoNode.GotoOperationTile != null;
            else if((!operation.TakesInput || InputNode.VariableTile != null) && 
                    (!operation.HasOutput || OutputNode.VariableTile != null))
                is_complete = true;

            if (is_complete)
            {
                Operation operation = Operation.Instantiate();
                if(operation.TakesInput && InputNode.VariableTile != null)
                    operation.Input.PrimaryVariableName = InputNode.VariableTile.Variable.Name;
                if(operation.HasOutput && OutputNode.VariableTile != null)
                    operation.Output.PrimaryVariableName = OutputNode.VariableTile.Variable.Name;
                if (operation.TakesGoto && GotoNode.GotoOperationTile != null)
                    operation.Goto = GotoNode.GotoOperationTile.Operation;

                Unit.Program.Add(operation);
                if(Input.GetKey(KeyCode.LeftShift))
                {
                    if(Unit.Program.Next == null)
                        Unit.Program.Next = operation;
                }
                else
                {
                    Unit.Program.Next = operation;

                    if (operation is Task)
                    {
                        Unit.Task = null;
                        operation.Execute(Unit);
                    }
                }

                InputNode.VariableName = null;
                InputNode.IsSelected = false;
                OutputNode.VariableName = null;
                OutputNode.IsSelected = false;
                GotoNode.GotoOperationTile = null;
                GotoNode.IsSelected = false;
                IsSelected = false;
            }
        }

        if (!InputUtility.DidDragOccur  &&
            this.IsPointedAt() && 
            !InputNode.IsPointedAt() && !OutputNode.IsPointedAt() && !GotoNode.IsPointedAt() && 
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

        if (InputUtility.WasMouseRightReleased)
        {
            if (IsSelected)
                IsSelected = false;
        }

        if(IsSelected)
        {
            if (Operation.TakesInput && InputNode.VariableTile == null && !Operation.TakesGoto)
            {
                if(!InputNode.IsSelected)
                    InputNode.IsSelected = true;
            }
            else if (Operation.HasOutput && OutputNode.VariableTile == null)
            {
                if(!OutputNode.IsSelected)
                    OutputNode.IsSelected = true;
            }
            else if (Operation.TakesGoto && GotoNode.GotoOperationTile == null)
            {
                if (!GotoNode.IsSelected)
                    GotoNode.IsSelected = true;
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
}

