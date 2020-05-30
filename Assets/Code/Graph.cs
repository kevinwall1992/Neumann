using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Graph
{
    HashSet<Node> nodes = new HashSet<Node>();

    public IEnumerable<Node> Nodes { get { return nodes; } }

    public IEnumerable<Edge> Edges
    {
        get
        {
            List<Edge> edges = new List<Edge>();

            foreach(Node node in Nodes)
                foreach(Node neighbor in node.Neighbors)
                    edges.Add(new Edge(node, neighbor));

            return edges;
        }
    }

    public float Connectivity
    {
        get
        {
            return Edges.Sum(edge => Nodes.Contains(edge.B) ? 1 : 0) / 
                   (float)Nodes.Count();
        }
    }


    public Graph(IEnumerable<Node> nodes_ = null)
    {
        if(nodes_ != null)
            nodes.UnionWith(nodes_);
    }

    public Node AddNode(Node node)
    {
        nodes.Add(node);

        return node;
    }

    public Node RemoveNode(Node node)
    {
        nodes.Remove(node);

        return node;
    }


    public class Node
    {
        public IData Data { get; set; }

        public IEnumerable<Node> Neighbors { get { return Data.GetNeighborsFunction(this); } }

        public Node(IData data)
        {
            Data = data;
        }


        public interface IData
        {
            System.Func<Node, IEnumerable<Node>> GetNeighborsFunction { get; }
            System.Func<Node, Node, float> GetCostFunction { get; }
        }
    }


    public class Edge : System.Tuple<Node, Node>
    {
        public Node A { get { return Item1; } }
        public Node B { get { return Item2; } }

        public Edge(Node a, Node b) : base(a, b) { }
    }


    public class Path : List<Node>
    {
        public IEnumerable<Edge> Edges
        {
            get
            {
                List<Edge> edges = new List<Edge>();

                for (int i = 0; i < this.Count - 1; i++)
                    edges.Add(new Edge(this[i], this[i + 1]));

                return edges;
            }
        }

        public Path(List<Node> nodes = null)
        {
            if (nodes != null)
                AddRange(nodes);
        }
    }


    public abstract class WritableNeighborData : Node.IData
    {
        public HashSet<Node> Neighbors { get; } = new HashSet<Node>();

        public System.Func<Node, IEnumerable<Node>> GetNeighborsFunction
        { get { return node => (node.Data as WritableNeighborData).Neighbors; } }

        public abstract System.Func<Node, Node, float> GetCostFunction { get; }
    }

    public class PositionData : WritableNeighborData
    {
        public virtual Vector3 Position { get; set; }

        public override System.Func<Node, Node, float> GetCostFunction
        {
            get
            {
                return (a, b) => (a.Data as PositionData).Position.Distance(
                                 (b.Data as PositionData).Position);
            }
        }

        public PositionData(Vector3 position, IEnumerable<Node> neighbors = null)
        {
            Position = position;

            if (neighbors != null)
                Neighbors.UnionWith(neighbors);
        }

        public PositionData()
        {
            
        }
    }

    public class SphereData : PositionData
    {
        public virtual float Radius { get; set; }

        public Sphere Sphere { get { return new Sphere(Position, Radius); } }

        public SphereData(Vector3 position, float radius, IEnumerable<Node> neighbors = null)
            : base(position, neighbors)
        {
            Radius = radius;
        }

        public SphereData()
        {

        }
    }

    public class UnitData : SphereData
    {
        public override Vector3 Position
        {
            get { return Unit.Physical.Position; }
            set { Unit.Physical.Position = value; }
        }

        public override float Radius
        {
            get { return Unit.Physical.Size; }
            set { Unit.Physical.Size = value; }
        }

        public Unit Unit { get; set; }

        public UnitData(Unit unit)
        {
            Unit = unit;
        }
    }
}


