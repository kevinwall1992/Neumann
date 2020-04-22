using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

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
        get
        {
            return variables;
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        variables = Memory.Variables.Merged(PlayerTeam.Stock.Variables);
    }

    public string MemorizePosition(Vector3 position)
    {
        string name = Memory.RememberName(position);
        if (name != null)
            return name;

        name = "Position " + Memory.Count<Vector3>();
        Memory.Memorize(name, position);

        return name;
    }

    
}
