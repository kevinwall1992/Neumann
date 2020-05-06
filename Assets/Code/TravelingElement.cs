using UnityEngine;
using System.Collections;

public class TravelingElement : MonoBehaviour
{
    public Transform OriginalParent { get; private set; }

    [SerializeField]
    Destination destination = Destination.None;

    void Start()
    {
        OriginalParent = transform.parent;

        switch (destination)
        {
            case Destination._3DUIElementsContainer:
                transform.SetParent(Scene.Main._3DUIElements.transform);
                break;

            case Destination.UnmaskedElementsContainer:
                transform.SetParent(Scene.Main.UnmaskedUIElements.transform);
                break;

            default:
                Debug.Assert(false, "TravelingElement did not have destination.");
                break;
        }
    }

    void Update()
    {
        
    }

    public enum Destination { None, _3DUIElementsContainer, UnmaskedElementsContainer }
}
