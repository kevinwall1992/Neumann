using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class PipeFunctionTile : Tile
{
    [SerializeField]
    Text name_text = null;

    public string Name { get; set; }
    public VariablePipe.PipeFunction Function { get; set; }

    protected override void Update()
    {
        base.Update();

        name_text.text = Name;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        PipeFunctionSlot[] pipe_function_slots =
            Scene.Main.UnmaskedUIElements.GetComponentsInChildren<PipeFunctionSlot>();

        if (pipe_function_slots.Length > 0)
        {
            PipeFunctionSlot pipe_function_slot =
                pipe_function_slots.Where(pipe_function_slot_ =>
                    (pipe_function_slot_.transform as RectTransform).ContainsMouse())
                    .FirstOrDefault();

            if (Drawer == null)
                GetComponentInParent<PipeFunctionSlot>().PipeFunctionTile = null;

            if (pipe_function_slot != null)
            {
                if (Drawer != null)
                {
                    PipeFunctionTile copy = Create(Name, Function);

                    Drawer.Add(copy);
                    copy.transform.SetSiblingIndex(transform.GetSiblingIndex());

                    Drawer.Remove(this);
                }
                
                pipe_function_slot.PipeFunctionTile = this;
            }
            else if(Drawer == null)
                GameObject.Destroy(gameObject);
        }

        base.OnEndDrag(eventData);
    }

    public static PipeFunctionTile Create(string name, VariablePipe.PipeFunction function)
    {
        PipeFunctionTile function_tile = GameObject.Instantiate(Scene.Main.Prefabs.PipeFunctionTile);
        function_tile.Name = name;
        function_tile.Function = function;

        return function_tile;
    }
}
