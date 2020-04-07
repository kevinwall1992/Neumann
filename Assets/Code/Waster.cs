using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Waster : MonoBehaviour, Able
{
    public Vector3 WasteSite;

    public bool WasteNot { get; set; }

    public IEnumerable<Operation> Abilities
    {
        get { return WasteNot ? null : Utility.List(new SelectWasteSiteOperation()); }
    }

    void Start()
    {
        WasteSite = transform.position + new Vector3(5, 0, 0);
        WasteNot = false;
    }

    void Update()
    {

    }
}

public class SelectWasteSiteOperation : Operation
{
    public override bool TakesInput { get { return true; } }

    public override void Execute(Unit unit)
    {
        if (unit.HasComponent<Waster>())
            unit.GetComponent<Waster>().WasteSite = Input.Read<Vector3>(unit);

        base.Execute(unit);
    }

    public override Operation Instantiate()
    {
        return new SelectWasteSiteOperation();
    }

    public override Style.Operation Style
    { get { return Scene.Main.Style.SelectWasteSiteOperation; } }
}
