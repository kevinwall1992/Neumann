using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Buildable : MonoBehaviour, HasVariables
{
    public Pile StructuralMaterials = new Pile();
    public Pile ProcessMaterials = new Pile();

    public Pile RequiredMaterials
    {
        get { return StructuralMaterials.Copy().PutIn(ProcessMaterials); }
    }

    [HideInInspector]
    public Pile Foundation;
    public float Completion
    {
        get
        {
            if (!IsProject || RequiredMaterials.Volume == 0)
                return 1;

            if (Foundation.Volume == 0)
                return 0;

            return Mathf.Min(1, Foundation.Resources.Min(resource => 
                Foundation.GetVolumeOf(resource) / 
                RequiredMaterials.GetVolumeOf(resource)));
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
                new FunctionVariable(Scene.Main.Style.VariableNames.Completion, 
                                     () => Completion));
        }
    }

    void Start()
    {
        
    }
    
    void Update()
    {
        if(IsProject && Foundation.Volume >= RequiredMaterials.Volume)
        {
            if(this.HasComponent<Unit>())
            GetComponent<Unit>().Team.Stock.Pile.PutIn(Foundation.Normalized() * 
                (Foundation.Volume - RequiredMaterials.Volume));
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
