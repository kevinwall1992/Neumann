using UnityEngine;
using System.Collections;

public class WorldInterface : UIElement
{
    [SerializeField]
    VariableDrawer variable_drawer = null;
    public VariableDrawer VariableDrawer { get { return variable_drawer; } }

    [SerializeField]
    Minimap minimap = null;
    public Minimap Minimap { get { return minimap; } }

    protected override void Start()
    {
        base.Start();

        VariableDrawer.VariableSource = Scene.Main.World;
    }

    protected override void Update()
    {
        base.Update();
    }
}
