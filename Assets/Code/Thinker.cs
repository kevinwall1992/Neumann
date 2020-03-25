using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Unit))]
public class Thinker : MonoBehaviour
{
    System.DateTime next_think;
    float leftover_thoughts = 0;

    public float ThinksPerSecond;
    public float ThoughtsPerSecond;

    public Unit Unit { get { return GetComponent<Unit>(); } }

    private void Start()
    {
        next_think = System.DateTime.Now;
    }

    private void Update()
    {
        if(!next_think.IsInFuture())
        {
            float thoughts = (ThoughtsPerSecond / ThinksPerSecond) + leftover_thoughts;
            int whole_thoughts = (int)thoughts;

            if(whole_thoughts > 0)
                Think(whole_thoughts);
            leftover_thoughts += thoughts - whole_thoughts;

            next_think = System.DateTime.Now.AddSeconds(1 / ThinksPerSecond);
        }
    }

    public virtual void Think(int thought_count)
    {
        while (thought_count-- > 0 && Unit.Program.Next != null)
        {
            Operation operation = Unit.Program.Next;

            operation.Execute(Unit);
        }
    }
}
