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

    void Start()
    {
        Line.material = new Material(Material);

        Color.Value = Color.Value.AlphaChangedTo(0);
    }

    void Update()
    {
        Vector3 local_target = transform.InverseTransformPoint(Target);

        for (int i = 0; i< Line.positionCount; i++)
            Line.SetPosition(i, Vector3.Lerp(Vector3.zero, local_target, i / (float)(Line.positionCount - 1)));

        Animator.speed = Mathf.Lerp(Animator.speed, AnimationBaseSpeed * NanolathingRate, 2 * Time.deltaTime);

        float target_alpha = NanolathingRate == 0 ? 0 : 1;
        Color.Value = Color.Value.AlphaChangedTo(
            Mathf.Lerp(Color.Value.a, 
                       target_alpha, 
                       (target_alpha > Color.Value.a ? 3 : 1) * Time.deltaTime));
    }
}
