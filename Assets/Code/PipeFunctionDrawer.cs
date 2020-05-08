using UnityEngine;
using System.Collections;

public class PipeFunctionDrawer : Drawer
{
    protected override void Start()
    {
        base.Start();

        if(UnityEditor.EditorApplication.isPlaying)
            foreach (string name in VariablePipe.Functions.Keys)
                Add(PipeFunctionTile.Create(name, VariablePipe.Functions[name]));
    }
}
