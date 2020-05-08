using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class TravelingElementContainer : MonoBehaviour
{
    public List<TravelingElement> TravelingElements
    { get { return GetComponentsInChildren<TravelingElement>(true).ToList(); } }

    void Start()
    {

    }

    void Update()
    {
        foreach (TravelingElement element in TravelingElements)
        {
            if (element.OriginalParent == null)
                GameObject.Destroy(element.gameObject);
            else
                element.gameObject.SetActive(element.OriginalParent.gameObject.activeSelf);
        }
    }
}
