using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class _3DUIElementContainer : MonoBehaviour
{
    public List<_3DUIElement> _3DUIElements { get { return GetComponentsInChildren<_3DUIElement>().ToList(); } }

    void Start()
    {

    }

    void Update()
    {
        foreach (_3DUIElement element in _3DUIElements)
        {
            if (element.OriginalParent == null)
                GameObject.Destroy(element.gameObject);
            else
                element.gameObject.SetActive(element.OriginalParent.gameObject.activeSelf);
        }
    }
}
