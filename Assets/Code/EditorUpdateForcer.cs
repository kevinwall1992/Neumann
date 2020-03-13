using UnityEngine;
using System.Collections;
using UnityEditor;

[ExecuteAlways]
public class EditorUpdateForcer : MonoBehaviour
{
    void Start()
    {

    }

    void Update()
    {
        EditorApplication.delayCall += EditorApplication.QueuePlayerLoopUpdate;
    }
}
