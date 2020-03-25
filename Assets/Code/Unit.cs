using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

[RequireComponent(typeof(Physical))]
[RequireComponent(typeof(Mortal))]
[RequireComponent(typeof(Buildable))]
public class Unit : MonoBehaviour, HasVariables
{
    public string Name;
    public Sprite Icon = null;
    public Color Color = Color.red;

    [SerializeField]
    GameObject selection_box = null;
    public GameObject SelectionBox { get { return selection_box; } }

    public List<Task> Abilities
    {
        get
        {
            return GetComponents<Able>().SelectMany(able => able.Abilities).ToList();
        }
    }

    Task task = null;
    public Task Task
    {
        get { return task; }
        set
        {
            task = value;

            if(task != null)
                task.Unit = this;
        }
    }

    public Program Program { get; set; }
    public bool IsSelected { get { return Scene.Main.UnitInterface.Unit == this; } }

    public Physical Physical { get { return GetComponent<Physical>(); } }
    public Mortal Mortal { get { return GetComponent<Mortal>(); } }
    public Buildable Buildable { get { return GetComponent<Buildable>(); } }

    public List<Variable> Variables
    {
        get
        {
            List<Variable> variables = new List<Variable>();

            variables.AddRange(Mortal.Variables);
            variables.AddRange(Buildable.Variables);

            return variables;
        }
    }

    void Start()
    {
        Program = new Program();
    }

    void Update()
    {
        if(SelectionBox != null)
            SelectionBox.SetActive(IsSelected);

        if (Task != null && Task.IsComplete)
            Task = null;

        if(InputUtility.WasMouseLeftReleased())
            if(this.IsTouched())
                Scene.Main.UnitInterface.Unit = this;
    }
}
