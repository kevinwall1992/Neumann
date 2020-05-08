using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class VariableDrawer : Drawer
{
    List<VariableTile> VariableTiles
    {
        get { return GetComponentsInChildren<VariableTile>().ToList(); }
    }

    HasVariables variable_source;
    public HasVariables VariableSource
    {
        get { return variable_source; }

        set
        {
            variable_source = value;

            Reset();
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (VariableSource == null)
            return;

        IEnumerable<Variable> variables = VariableTiles.Select(variable_tile => variable_tile.Variable);

        foreach (Variable variable in VariableSource.Variables)
            if(!variables.Contains(variable))
                Add(VariableTile.Create(variable));

        foreach (VariableTile variable_tile in VariableTiles)
            if (!VariableSource.Variables.Contains(variable_tile.Variable))
            {
                Remove(variable_tile);
                GameObject.Destroy(variable_tile.gameObject);
            }
    }

    void SetVariableContainer(HasVariables variable_container)
    {
        VariableSource = variable_container;
    }
}
