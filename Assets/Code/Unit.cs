using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

[RequireComponent(typeof(Physical))]
[RequireComponent(typeof(Mortal))]
[RequireComponent(typeof(Buildable))]
[RequireComponent(typeof(Thinker))]
public class Unit : MonoBehaviour, HasVariables
{
    public string Name;
    public Sprite Icon = null;

    public Memory Memory { get; private set; } = new Memory();

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
    public Task Task { get; set; }

    public Team Team { get { return GetComponentInParent<Team>(); } }
    public Program Program { get; set; }
    public bool IsSelected { get { return Scene.Main.UnitInterface.Unit == this; } }

    public Physical Physical { get { return GetComponent<Physical>(); } }
    public Mortal Mortal { get { return GetComponent<Mortal>(); } }
    public Buildable Buildable { get { return GetComponent<Buildable>(); } }
    public Thinker Thinker { get { return GetComponent<Thinker>(); } }

    public List<Variable> Variables
    {
        get
        {
            List<Variable> variables = new List<Variable>();
            variables.Add(new FunctionVariable("Busy", () => Task != null));

            variables.AddRange(Mortal.Variables);
            variables.AddRange(Buildable.Variables);

            variables.AddRange(Memory.Variables);

            return variables;
        }
    }

    void Start()
    {
        Program = new Program();
        Memory.Memorize("Variable", 0, true);
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
