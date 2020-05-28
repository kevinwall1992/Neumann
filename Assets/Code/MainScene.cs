using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.EventSystems;


public class MainScene : Scene
{
    OperationTile cursor_operation_tile = null;

    public Camera Camera;
    public Canvas Canvas;
    public EventSystem EventSystem;
    public MainInputModule InputModule;
    public World World;
    public Style Style;
    public TravelingElementContainer _3DUIElements;
    public Prefabs Prefabs;
    public UnitInterface UnitInterface;
    public WorldInterface WorldInterface;
    public TravelingElementContainer UnmaskedUIElements;

    private void Awake()
    {
        
    }

    void Start()
    {
        Cursor.Default.Use();
    }

    void Update()
    {
        if (OperationTile.Selected != null)
        {
            if (OperationTile.Selected != cursor_operation_tile)
            {
                if(OperationTile.Selected.Operation.Style.Cursor != null)
                    OperationTile.Selected.Operation.Style.Cursor.Use();

                cursor_operation_tile = OperationTile.Selected;
            }
        }
        else if (Cursor.Current != Cursor.Default)
        {
            Cursor.Default.Use();
            cursor_operation_tile = null;
        }

        if (this.IsModulusUpdate(60))
            GraphUtility.ClearMetricTable();
    }
}
