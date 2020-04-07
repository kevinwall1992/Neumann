using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Memory : HasVariables
{
    Dictionary<string, object> memories = new Dictionary<string, object>();
    HashSet<string> writable_memories = new HashSet<string>();

    Dictionary<object, string> memory_names = new Dictionary<object, string>();

    Dictionary<string, Variable> variables = new Dictionary<string, Variable>();

    public List<Variable> Variables
    {
        get
        {
            List<Variable> new_variables = new List<Variable>();

            new_variables.AddRange(memories.Keys
                .Where(name => writable_memories.Contains(name) && !variables.ContainsKey(name))
                .Select(name => new WritableVariable(name, memories[name])));

            new_variables.AddRange(memories.Keys
                .Where(name => !writable_memories.Contains(name) && !variables.ContainsKey(name))
                .Select(name => memories[name] is System.Func<object> ? 
                                (Variable)(new FunctionVariable(name, memories[name] as System.Func<object>)) : 
                                (Variable)(new ReadOnlyVariable(name, memories[name]))));

            foreach (Variable variable in new_variables)
                variables[variable.Name] = variable;

            foreach (string name in variables.Keys.ToList())
                if (!memories.ContainsKey(name))
                    variables.Remove(name);

            return variables.Values.ToList();
        }
    }

    public void Memorize(string name, object memory, bool is_writable = false)
    {
        if(memory is System.Delegate && !(memory is System.Func<object>))
        {
            System.Delegate delegate_ = memory as System.Delegate;
            if (delegate_.Method.GetParameters().Count() == 0)
                memory = (System.Func<object>)(() => delegate_.DynamicInvoke());
        }

        memories[name] = memory;
        memory_names[memory] = name;

        if (is_writable)
            writable_memories.Add(name);
        else if (writable_memories.Contains(name))
            writable_memories.Remove(name);
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

    //Remembers the last Memorize()d name for memory
    //e.g. Storing the number "7" twice means the second
    //name you gave it will be returned here.
    public string RememberName(object memory)
    {
        if (!memory_names.ContainsKey(memory))
            return null;

        return memory_names[memory];
    }

    public int Count<T>()
    {
        return memories.Values.Where(value => value is T).Count();
    }
}
