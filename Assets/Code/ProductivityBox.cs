using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ProductivityBox : MonoBehaviour
{
    [SerializeField]
    Text productivity_text;

    [SerializeField]
    Image image, divider;

    [SerializeField]
    CanvasGroup canvas_group;

    void Start()
    {

    }

    void Update()
    {
        if(OperationTile.Selected == null || !(OperationTile.Selected.Operation is TransportEfficiencyTask))
        {
            canvas_group.alpha = 0;
            return;
        }

        canvas_group.alpha = Mathf.Lerp(canvas_group.alpha, 1, Time.deltaTime * 10);

        TransportEfficiencyTask transport_efficiency_task =
            OperationTile.Selected.Operation as TransportEfficiencyTask;
        float transport_efficiency = transport_efficiency_task
            .GetTransportEfficiency(Scene.Main.World.Asteroid.GetWorldPositionPointedAt()
                .Distance(Scene.Main.UnitInterface.Unit.Physical.Position));

        productivity_text.text = transport_efficiency.ToString("P0");

        Color color = Color.white.Lerped(Color.red, 1 - transport_efficiency);
        image.color = divider.color = color;

        transform.position = Input.mousePosition;
    }
}
