using UnityEngine;
using System.Collections;

public class _3DUIElement : MonoBehaviour
{
    public Transform OriginalParent { get; private set; }

    void Start()
    {
        OriginalParent = transform.parent;
        transform.parent = Scene.Main._3DUIElementContainer.transform;
    }

    void Update()
    {
        
    }
}
