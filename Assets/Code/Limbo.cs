using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Limbo : MonoBehaviour
{
    List<Mortal> souls = new List<Mortal>();

    public IEnumerable<Mortal> Souls { get; }

    void Start()
    {

    }

    void Update()
    {
        List<Mortal> newly_departed = new List<Mortal>(
            Scene.Main.World
            .GetComponentsInChildren<Mortal>()
            .Where(mortal => mortal.IsDead));

        foreach(Mortal mortal in newly_departed)
        {
            mortal.gameObject.SetActive(false);
            mortal.transform.SetParent(transform);
        }

        souls.AddRange(newly_departed);
    }
}
