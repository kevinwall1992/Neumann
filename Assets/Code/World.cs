using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Physics))]
public class World : MonoBehaviour, HasVariables
{
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

    public List<Variable> Variables
    {
        get
        {
            return Memory.Variables.Merged(PlayerTeam.Stock.Variables);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(OperationTile.Selected == null && 
           InputUtility.WasMouseRightReleased() && 
           !InputUtility.DidDragOccur() &&
           Asteroid.TerrainCollider.gameObject.IsTouched())
            Scene.Main.UnitInterface.Unit = null;
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
