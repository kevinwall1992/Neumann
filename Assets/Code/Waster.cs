using UnityEngine;
using System.Collections;

public class Waster : MonoBehaviour
{
    public Vector3 WasteSite;

    void Start()
    {
        WasteSite = transform.position + new Vector3(5, 0, 0);
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
