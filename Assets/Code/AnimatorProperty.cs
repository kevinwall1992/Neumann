using UnityEngine;
using System.Collections;

[System.Serializable]
public abstract class AnimatorProperty<T>
{
    [SerializeField]
    Animator animator = null;
    [SerializeField]
    string name = null;

    protected Animator Animator { get { return animator; } }
    protected string Name { get { return name; } }

    public abstract T Value { get; set; }
}

[System.Serializable]
public class FloatAnimatorProperty : AnimatorProperty<float>
{
    public override float Value
    {
        get { return Animator.GetFloat(Name); }
        set { Animator.SetFloat(Name, value); }
    }
}

[System.Serializable]
public class BoolAnimatorProperty : AnimatorProperty<bool>
{
    public override bool Value
    {
        get { return Animator.GetBool(Name); }
        set { Animator.SetBool(Name, value); }
    }
}

