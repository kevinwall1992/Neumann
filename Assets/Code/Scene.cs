using UnityEngine;
using System.Collections;

public class Scene : MonoBehaviour
{
    static MainScene main_scene = null;
    public static MainScene Main
    {
        get
        {
            if (main_scene == null)
                main_scene = GameObject.FindObjectOfType<MainScene>();

            return main_scene;
        }
    }
}
