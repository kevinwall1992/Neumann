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

    public List<Operation> Abilities
    {
        get
        {
            return GetComponents<Able>().SelectMany(able => able.Abilities).ToList();
        }
    }

    public Team Team { get { return GetComponentInParent<Team>(); } }
    public Task Task { get; set; }
    public Program Program { get; set; } = new Program();
    public bool IsSelected { get { return Scene.Main.UnitInterface.Unit == this; } }

    public List<Variable> Variables
    {
        get
        {
            List<Variable> variables = new List<Variable>();

            variables.AddRange(Mortal.Variables);
            variables.AddRange(Buildable.Variables);

            variables.AddRange(Memory.Variables);

            return variables;
        }
    }

    public Physical Physical { get { return GetComponent<Physical>(); } }
    public Mortal Mortal { get { return GetComponent<Mortal>(); } }
    public Buildable Buildable { get { return GetComponent<Buildable>(); } }

    
    void Start()
    {
        Memory.Memorize("Variable", 0, true);

        Memory.Memorize("Busy", () => Task != null);

        Memory.Memorize("Iron Concentration", 
            () => 100 * Scene.Main.World.Asteroid.Regolith
                .GetConcentrationByVolume(Physical.Position, 1, new Resource("Iron")));

        Memory.Memorize("Deep Iron Concentration", 
            () => 100 * Scene.Main.World.Asteroid.Rock
                .GetConcentrationByVolume(Physical.Position, 1, new Resource("Iron")));
    }

    void Update()
    {
        if(SelectionBox != null)
            SelectionBox.SetActive(IsSelected);

        if (Task != null && Task.IsComplete)
            Task = null;

        if(this.UseMouseLeftRelease())
            if(this.IsPointedAt())
                if(Scene.Main.UnitInterface.Unit == null || 
                   !Scene.Main.UnitInterface.Unit.IsPointedAt() || 
                   (this.HasComponent<Motile>() && 
                        !Scene.Main.UnitInterface.Unit.HasComponent<Motile>()))
                    Scene.Main.UnitInterface.Unit = this;
    }
}
