using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NanolathingLineController : MonoBehaviour
{
    public LineRenderer Line;
    public Material Material;
    public Animator Animator;
    public float AnimationBaseSpeed;
    public ColorMaterialProperty Color;

    public float NanolathingRate { get; set; }
    public Vector3 Target { get; set; }

    public bool IsVisible
    {
        get { return gameObject.activeSelf; }
        set { gameObject.SetActive(value); }
    }

    void Start()
    {
        Line.material = new Material(Material);
    }

    void Update()
    {
        Vector3 local_target = transform.InverseTransformPoint(Target);

        for (int i = 0; i< Line.positionCount; i++)
            Line.SetPosition(i, Vector3.Lerp(Vector3.zero, local_target, i / (float)(Line.positionCount - 1)));

        Animator.speed = Mathf.Lerp(Animator.speed, AnimationBaseSpeed * NanolathingRate, 2 * Time.deltaTime);

        if(NanolathingRate == 0)
            Color.Value = Color.Value.AlphaChangedTo(Mathf.Lerp(Color.Value.a, 0, 1 * Time.deltaTime));
    }
}
