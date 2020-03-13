using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Prefabs : MonoBehaviour
{
    public VariableTile VariableTile;
    public OperationTile OperationTile;

    [SerializeField]
    List<Unit> units = new List<Unit>();
    public Dictionary<string, Unit> Units
    {
        get { return units.ToDictionary(unit => unit.Name, unit => unit); }
    }

    void Start()
    {

    }

    void Update()
    {

    }
}
