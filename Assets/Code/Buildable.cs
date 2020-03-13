using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Buildable : MonoBehaviour, HasVariables
{
    public Pile RequiredResources = new Pile();
    public Pile RetainedResources = new Pile();
    public string CompletionVariableName = "% Complete";

    [HideInInspector]
    public Pile Foundation;
    public float Completion
    {
        get
        {
            if (!IsProject || RequiredResources.Volume == 0)
                return 1;

            if (Foundation.Volume == 0)
                return 0;

            return Mathf.Min(1, Foundation.Resources.Min(resource => 
                Foundation.GetVolumeOf(resource) / 
                RequiredResources.GetVolumeOf(resource)));
        }
    }
    public bool IsProject { get; set; }

    public List<Variable> Variables
    {
        get
        {
            if (!IsProject)
                return new List<Variable>();

            return Utility.List<Variable>(
                new FunctionVariable(CompletionVariableName, () => Completion * 100)
                .Stylize(Scene.Main.Style.Variables[CompletionVariableName]));
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if(IsProject && Foundation.Volume >= RequiredResources.Volume)
        {
            Scene.Main.World.Stock.Pile.PutIn(Foundation.Normalized() * 
                                              (Foundation.Volume - RequiredResources.Volume));
            IsProject = false;
        }
    }

    public void Construct(Pile materials)
    {
        Foundation.PutIn(materials);
    }

    public Buildable InstantiateProject(Vector3 construction_site)
    {
        Buildable copy = GameObject.Instantiate(this, Scene.Main.World.transform);
        copy.transform.position = construction_site;

        copy.Foundation = new Pile();
        copy.IsProject = true;

        return copy;
    }
}
