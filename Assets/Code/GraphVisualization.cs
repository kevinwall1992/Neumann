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
            Color color = GetEdgeColor(edge);

            ArrowLine arrow_line = GameObject.Instantiate(Scene.Main.Prefabs.ArrowLine);
            arrow_line.transform.parent = transform;

            arrow_line.Line.useWorldSpace = true;
            arrow_line.Line.SetPosition(0, ApplyOffset(edge.A.GetPosition()));
            arrow_line.Line.SetPosition(1, ApplyOffset(edge.B.GetPosition()));

            arrow_line.Line.startWidth = arrow_line.Line.endWidth = GetEdgeWidth(edge);

            ColorMaterialProperty.SetValueOfOther(arrow_line.Line, color);

            arrow_line.Arrow.Color = color;
            arrow_line.Arrow.ArrowSize = GetEdgeWidth(edge) * 6;
        }

        is_graph_invalid = false;
    }
}
