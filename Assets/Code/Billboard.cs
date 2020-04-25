using UnityEngine;
using System.Collections;


public class Billboard : MonoBehaviour
{
    void Update()
    {
        transform.localRotation = 
            Quaternion.LookRotation(transform.position - Scene.Main.Camera.transform.position, 
                                    Scene.Main.Camera.transform.up);
    }
}
