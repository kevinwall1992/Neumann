using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Memory : HasVariables
{
    Dictionary<string, Variable> memories = new Dictionary<string, Variable>();
    Dictionary<Variable, string> memory_names = new Dictionary<Variable, string>();

    public List<Variable> Variables { get { return memories.Values.ToList(); } }


    public void Memorize(string name, object memory, bool is_writable = false)
    {
        if(memory is System.Delegate && !(memory is System.Func<object>))
        {
            System.Delegate delegate_ = memory as System.Delegate;
            if (delegate_.Method.GetParameters().Count() == 0)
                memory = (System.Func<object>)(() => delegate_.DynamicInvoke());
        }

        if (is_writable)
            memories[name] = new WritableVariable(name, memory);
        else if (memory is System.Func<object>)
            memories[name] = new FunctionVariable(name, memory as System.Func<object>);
        else
            memories[name] = new ReadOnlyVariable(name, memory);

        memory_names[memories[name]] = name;
    }

    public void Memorize(string name, System.Func<object> memory)
    {
        Memorize(name, memory, false);
    }

    public object Remember(string name)
    {
        if (!memories.ContainsKey(name))
            return null;

        return memories[name];
    }

    public void Forget(string name)
    {
        memories.Remove(name);
    }
}
