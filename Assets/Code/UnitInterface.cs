using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class UnitInterface : UIElement
{
    [SerializeField]
    RectTransform unit_info = null;

    [SerializeField]
    RectTransform program_interface_container = null;

    [SerializeField]
    RectTransform next_operation_tile_mirror_axis = null;


    [SerializeField]
    Text name_text = null;
    Text NameText { get { return name_text; } }

    [SerializeField]
    Image image = null;
    Image Image { get { return image; } }

    [SerializeField]
    OperationTile current_task_tile = null;
    OperationTile CurrentTaskTile { get { return current_task_tile; } }

    [SerializeField]
    OperationTile next_operation_tile = null;
    OperationTile NextOperationTile { get { return next_operation_tile; } }

    [SerializeField]
    CanvasGroup canvas_group = null;
    CanvasGroup CanvasGroup { get { return canvas_group; } }

    [SerializeField]
    VariableDrawer variable_drawer = null;
    public VariableDrawer VariableDrawer { get { return variable_drawer; } }

    [SerializeField]
    OperationMenu operation_menu = null;
    public OperationMenu OperationMenu { get { return operation_menu; } }

    [SerializeField]
    ProgramInterface program_interface = null;
    public ProgramInterface ProgramInterface { get { return program_interface; } }


    Unit unit;
    public Unit Unit
    {
        get { return unit; }

        set
        {
            unit = value;

            if (unit != null)
            {
                NameText.text = Unit.Name;

                Image.sprite = Unit.Icon;
                image.color = Unit.Team.Color;

                VariableDrawer.VariableSource = Unit;
                OperationMenu.Abilities = Unit.Abilities;
                ProgramInterface.Program = Unit.Program;

                Show();
            }
            else
            {
                VariableDrawer.VariableSource = null;
                OperationMenu.Abilities = null;
                ProgramInterface.Program = null;
                Hide();
            }
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (Unit!= null && Unit.Mortal.IsDead)
            Unit = null;

        if (Unit == null)
        {
            Hide();
            return;
        }

        if ((CurrentTaskTile.Operation == null && Unit.Task != null) || 
            (CurrentTaskTile.Operation != null && (CurrentTaskTile.Operation as Task) != Unit.Task))
        {
            if (Unit.Task != null)
                CurrentTaskTile.Operation = Unit.Task;
            else
                CurrentTaskTile.Operation = null;
        }


        float space = unit_info.rect.height * 0.25f + Scene.Main.Style.Padding;
        bool show_next_operation_tile = !ProgramInterface.IsOpen && 
                                        Unit.Program.Next != null && 
                                        !ProgramInterface.Handle.IsBeingDragged;

        Vector3 target_position = 
            new Vector3(next_operation_tile_mirror_axis.position.x, 
                        next_operation_tile_mirror_axis.position.y + 
                            (show_next_operation_tile ? -1 : 1) * space / 2);
        NextOperationTile.transform.position = NextOperationTile.transform.position
            .Lerped(target_position, Time.deltaTime * 4);

        if (show_next_operation_tile)
        {
            NextOperationTile.Operation = Unit.Program.Next;

            program_interface_container.sizeDelta = 
                Vector2.Lerp(program_interface_container.sizeDelta,
                             new Vector2(program_interface_container.sizeDelta.x, -space), 
                             Time.deltaTime * 4);
        }
        else
        {
            program_interface_container.sizeDelta =
                new Vector2(program_interface_container.sizeDelta.x, 0);
        }


        if (OperationTile.Selected == null && 
            OperationTileIONode.Selected == null && 
            InputUtility.WasMouseLeftPressed && 
            Scene.Main.World.IsPointedAt())
        {
            foreach (OperationTile operation_tile in operation_menu.OperationTiles)
                if (operation_tile.Operation is MoveTask)
                {
                    operation_tile.IsSelected = true;
                    break;
                }
        }

        if (InputUtility.WasMouseRightReleased)
            Unit = null;
    }

    public void Hide()
    {
        CanvasGroup.alpha = 0;
        CanvasGroup.blocksRaycasts = false;
        CanvasGroup.interactable = false;
    }

    public void Show()
    {
        CanvasGroup.alpha = 1;
        CanvasGroup.blocksRaycasts = true;
        CanvasGroup.interactable = true;
    }
}
