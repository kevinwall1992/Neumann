using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

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
                Add(value);

            next_index = IndexOf(value);
        }
    }

    public IEnumerable<Operation> GetSegment(Operation local_operation)
    {
        Debug.Assert(Contains(local_operation), "Program doesn't contain local_operation");

        int segment_start, segment_end;
        segment_start = segment_end = IndexOf(local_operation);

        while (segment_start > 0 && !(this[segment_start] is ChooseOperation))
            segment_start--;

        while (segment_end < (Count - 1) && !(this[segment_end] is ChooseOperation))
            segment_end++;

        return GetRange(segment_start, segment_end - segment_start + 1);
    }
}
