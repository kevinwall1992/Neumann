using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Mortal : MonoBehaviour, HasVariables
{
    public FillableAttribute Health = new FillableAttribute("Health", 100);

    public bool IsDead { get { return Health.Value <= 0; } }

    public List<Variable> Variables { get { return Health.Variables; } }

    void Start()
    {
        
    }

    void Update()
    {

    }
}
