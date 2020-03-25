using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Program : List<Operation>
{
    int next_index = -1;

    public Operation Next
    {
        get
        {
            if (next_index < 0 || next_index >= Count)
            {
                next_index = -1;
                return null;
            }

            return this[next_index];
        }

        set
        {
            if (value == null)
                next_index = -1;
            else if (!Contains(value))
            {
                Debug.Assert(false);
                return;
            }

            next_index = IndexOf(value);
        }
    }
}
