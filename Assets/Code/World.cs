using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

[RequireComponent(typeof(Physics))]
public class World : MonoBehaviour, HasVariables
{
    public float PhysicalScale;

    [SerializeField]
    Asteroid asteroid = null;
    public Asteroid Asteroid { get { return asteroid; } }

    [SerializeField]
    Physics physics = null;
    public Physics Physics { get { return physics; } }

    [SerializeField]
    Team player_team = null;
    public Team PlayerTeam { get { return player_team; } }

    public Memory Memory { get; private set; } = new Memory();

    List<Variable> variables = new List<Variable>();
    public List<Variable> Variables
    {
        get { return variables; }
    }

    public IEnumerable<Unit> Units { get { return GetComponentsInChildren<Unit>(); } }
    public IEnumerable<Motile> Motiles { get { return GetComponentsInChildren<Motile>(); } }
    public IEnumerable<Unit> Buildings
    {
        get
        {
            return GetComponentsInChildren<Unit>()
                .Where(unit => !(unit.HasComponent<Motile>()));
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        variables = Memory.Variables.Merged(PlayerTeam.Stock.Variables);
    }

    int position_count = 0;
    public string MemorizePosition(Vector3 position)
    {
        name = "Position " + position_count++;
        Memory.Memorize(name, position);

        return name;
    }
}
