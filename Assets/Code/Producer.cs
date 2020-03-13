using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Unit))]
public class Producer : MonoBehaviour
{
    public Pile Feedstock = new Pile();
    public Pile Product = new Pile();

    void Start()
    {

    }

    void Update()
    {

    }
}


public class ProductionTask : Task
{
    public override bool IsComplete { get { return false; } }

    public ProductionTask()
    {

    }


    public override Style.Operation Style
    { get { return Scene.Main.Style.ProductionTask; } }
}
