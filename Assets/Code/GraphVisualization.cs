using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class GraphVisualization : MonoBehaviour
{
    Graph graph = null;
    bool is_graph_invalid = true;

    public Graph Graph
    {
        get { return graph; }

        set
        {
            if (value != null)
                graph = value.Copy();
            else
                graph = null;

            is_graph_invalid = true;
        }
    }

    public Material Material;
    public ColorMaterialProperty ColorMaterialProperty;

    public System.Func<Graph.Edge, float> GetEdgeWidth { get; set; } = edge => 10;

    public System.Func<Graph.Edge, Color> GetEdgeColor { get; set; } = edge => Color.green;

    public System.Func<Vector3, Vector3> ApplyOffset { get; set; } = position => position;

    public IEnumerable<LineRenderer> Lines
    { get { return GetComponentsInChildren<LineRenderer>(); } }

    protected virtual void Update()
    {
        if (!is_graph_invalid || graph == null)
            return;

        foreach(LineRenderer line in Lines.ToList())
            GameObject.Destroy(line.gameObject);

        foreach(Graph.Edge edge in graph.Edges)
        {
            LineRenderer line = new GameObject("GraphLine").AddComponent<LineRenderer>();
            line.transform.parent = transform;
            line.useWorldSpace = true;

            line.material = Material;
            ColorMaterialProperty.SetValueOfOther(line, GetEdgeColor(edge));

            line.positionCount = 2;
            line.SetPosition(0, ApplyOffset(edge.A.GetPosition()));
            line.SetPosition(1, ApplyOffset(edge.B.GetPosition()));
        }

        is_graph_invalid = false;
    }
}
