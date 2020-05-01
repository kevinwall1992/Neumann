using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class UnitInterface : UIElement
{
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
