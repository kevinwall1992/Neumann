using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Graph
{
    HashSet<Node> nodes = new HashSet<Node>();

    public IEnumerable<Node> Nodes { get { return nodes; } }

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

    public Node GetNearestNode(System.Func<Node, float> metric)
    {
        return nodes.MinElement(node => metric(node));
    }

    public Node GetNearestNode(Node node)
    {
        return GetNearestNode(node_ => node_.Cost(node));
    }

    public Node GetNearestNode(Vector3 position)
    {
        return GetNearestNode(node => (node.Data as PositionData).Position.Distance(position));
    }

    public Graph Defleshed(Graph xray)
    {
        Dictionary<Node, Node> xray_analogs = new Dictionary<Node, Node>();

        foreach (Node node in xray.Nodes)
            xray_analogs[node] = GetNearestNode(node);

        HashSet<Node> skeleton = new HashSet<Node>();

        foreach (Node node in xray.Nodes)
            foreach (Node neighbor in node.Neighbors)
                foreach (Node path_node in xray_analogs[node].GetPathTo(xray_analogs[neighbor]))
                    skeleton.Add(path_node);

        return new Graph(skeleton);
    }

    public Graph Difference(Graph other)
    {
        Graph difference = new Graph(nodes);

        foreach(Node node in nodes)
            if (other.Nodes.Contains(node))
                difference.nodes.Remove(node);

        return difference;
    }

    public Graph Union(Graph other)
    {
        Graph union = new Graph(nodes);

        foreach (Node node in other.nodes)
            if (!union.nodes.Contains(node))
                union.nodes.Add(node);

        return union;
    }

    //Requires PositionData
    public Graph MinimumSpanned()
    {
        HashSet<Node> open_set = Copy().nodes;

        Graph mst = new Graph();
        mst.AddNode(open_set.First());
        open_set.Remove(open_set.First());

        while (open_set.Count > 0)
        {
            List<System.Tuple<Node, Node>> edges = new List<System.Tuple<Node, Node>>();

            foreach (Node closed_node in mst.Nodes)
                foreach (Node neighbor in closed_node.Neighbors)
                    if (open_set.Contains(neighbor))
                        edges.Add(new System.Tuple<Node, Node>(closed_node, neighbor));

            System.Tuple<Node, Node> minimum_edge = edges.MinElement(edge => edge.Item1.Cost(edge.Item2));

            Node node = minimum_edge.Item2;
            foreach (Node neighbor in new List<Node>(node.Neighbors))
                if (mst.Nodes.Contains(neighbor) && neighbor != minimum_edge.Item1)
                    (node.Data as WritableNeighborData).Neighbors.Remove(neighbor);

            open_set.Remove(node);
            mst.AddNode(node);

            foreach (Node tree_node in mst.Nodes)
                if (tree_node != minimum_edge.Item1 && tree_node.Neighbors.Contains(node))
                    (tree_node.Data as PositionData).Neighbors.Remove(node);
        }

        return mst;
    }

    public Graph WithoutHiddenNodes()
    {
        return Copy(true);
    }

    //Assumes PositionData
    public Graph Copy(bool remove_hidden_nodes = false)
    {
        HashSet<Node> nodes;

        if (!remove_hidden_nodes)
        {
            nodes = new HashSet<Node>();

            HashSet<Node> open_set = new HashSet<Node>();
            open_set.UnionWith(this.nodes);

            while(open_set.Count > 0)
            {
                Node node = open_set.First();
                open_set.Remove(node);
                nodes.Add(node);

                foreach (Node neighbor in node.Neighbors)
                    if (!nodes.Contains(neighbor))
                        open_set.Add(neighbor);
            }
        }
        else
            nodes = this.nodes;


        Graph copy = new Graph();

        Dictionary<Node, Node> copy_map = new Dictionary<Node, Node>();

        foreach (Node node in nodes)
        {
            Node node_copy = new Node(new PositionData((node.Data as PositionData).Position));

            copy.AddNode(node_copy);
            copy_map[node] = node_copy;
        }

        Dictionary<Node, Node> original_map = copy_map.Inverted();

        foreach (Node node in copy.Nodes)
            foreach (Node neighbor in original_map[node].Neighbors)
                if (copy_map.ContainsKey(neighbor))
                    (node.Data as PositionData).Neighbors.Add(copy_map[neighbor]);

        return copy;
    }


    public static Graph CreateGrid(float width, float height, int resolution, 
                                 Vector3 offset = new Vector3())
    {
        Graph graph = new Graph();

        Node[,] grid = new Node[resolution, resolution];

        for(int x = 0; x < resolution; x++)
        {
            float normalized_x = x / (resolution - 1.0f);

            for(int y = 0; y < resolution; y++)
            {
                float normalized_y = y / (resolution - 1.0f);

                graph.AddNode(grid[x, y] = 
                    new Node(new PositionData(new Vector3(width * normalized_x, 
                                                          0, 
                                                          height * normalized_y) + 
                                              offset)));
            }
        }


        Vector2Int[] neighbor_displacements = { new Vector2Int(-1, -1),
                                                new Vector2Int(-1, 0),
                                                new Vector2Int(-1, 1),
                                                new Vector2Int(0, -1),

                                                new Vector2Int(0, 1),
                                                new Vector2Int(1, -1),
                                                new Vector2Int(1, 0),
                                                new Vector2Int(1, 1) };

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                Node node = grid[x, y];

                foreach(Vector2Int displacement in neighbor_displacements)
                {
                    Vector2Int index = new Vector2Int(x, y) + displacement;

                    if (index.x >= 0 && index.x < resolution && 
                        index.y >= 0 && index.y < resolution)
                        (node.Data as PositionData).Neighbors.Add(grid[index.x, index.y]);
                }
            }
        }

        return graph;
    }

    public static Graph CreateGrid(Terrain terrain)
    {
        float width = terrain.terrainData.size.x;
        float height = terrain.terrainData.size.y;

        Graph grid = CreateGrid(width, height, (int)(Mathf.Max(width, height) / 5), terrain.GetPosition());

        foreach (Node node in grid.Nodes)
        {
            PositionData data = (node.Data as PositionData);

            data.Position = data.Position.YChangedTo(terrain.SampleHeight(data.Position));
        }

        return grid;
    }

    public static Graph CreateHairball(IEnumerable<Vector3> positions)
    {
        List<Node> nodes = new List<Node>();

        foreach (Vector3 position in positions)
            nodes.Add(new Node(new PositionData(position)));

        foreach (Node node in nodes)
            foreach (Node other_node in nodes)
                if (node != other_node)
                    (node.Data as PositionData).Neighbors.Add(other_node);

        return new Graph(nodes);
    }


    public class Node
    {
        public IData Data { get; set; }

        public IEnumerable<Node> Neighbors { get { return Data.GetNeighborsFunction(this); } }

        public Node(IData data)
        {
            Data = data;
        }

        //Assumes admissible, consistant heuristic
        public Path GetPathTo(Node destination)
        {
            Node source = this;

            Dictionary<Node, Node> previous_node = new Dictionary<Node, Node>();
            Dictionary<Node, float> cost_to_source = new Dictionary<Node, float>();
            Dictionary<Node, float> cost_to_destination_estimate = new Dictionary<Node, float>();

            System.Func<Node, float> GetPathCostEstimate = 
                node => cost_to_source[node] + cost_to_destination_estimate[node];

            SortedSet <Node> open_set = new SortedSet<Node>(
                Comparer<Node>.Create((a, b) => GetPathCostEstimate(a).CompareTo(GetPathCostEstimate(b))));
            HashSet<Node> closed_set = new HashSet<Node>();

            open_set.Add(source);
            previous_node[source] = null;
            cost_to_source[source] = 0;
            cost_to_destination_estimate[source] = 0;

            while(open_set.Count > 0)
            {
                List<System.Tuple<Node, float>> node_estimates = new List<System.Tuple<Node, float>>();
                foreach (Node node_ in open_set)
                    node_estimates.Add(new System.Tuple<Node, float>(node_, GetPathCostEstimate(node_)));

                Node node = open_set.First();
                open_set.Remove(node);
                closed_set.Add(node);

                if (node == destination)
                    break;

                foreach(Node neighbor in node.Neighbors)
                {
                    if (closed_set.Contains(neighbor))
                        continue;

                    float neighbor_to_source_cost = node.Cost(neighbor) + cost_to_source[node];

                    bool contains = cost_to_source.ContainsKey(neighbor);

                    if (cost_to_source.ContainsKey(neighbor))
                    {
                        if (neighbor_to_source_cost > cost_to_source[neighbor])
                            continue;
                        else
                            open_set.Remove(neighbor);
                    }

                    previous_node[neighbor] = node;
                    cost_to_source[neighbor] = neighbor_to_source_cost;
                    cost_to_destination_estimate[neighbor] = neighbor.Cost(destination);

                    open_set.Add(neighbor);
                }
            }

            return new Path(Utility.List(destination, node => previous_node[node]).Reversed());
        }

        //Should return real cost when parameter is a neighbor,
        //And return admissible heuristic of path cost when it isn't
        public float Cost(Node other) { return Data.GetCostFunction(this, other); }


        public interface IData
        {
            System.Func<Node, IEnumerable<Node>> GetNeighborsFunction { get; }
            System.Func<Node, Node, float> GetCostFunction { get; }
        }
    }

    public abstract class WritableNeighborData : Node.IData
    {
        public HashSet<Node> Neighbors { get; } = new HashSet<Node>();

        public System.Func<Node, IEnumerable<Node>> GetNeighborsFunction
        { get { return node => (node.Data as PositionData).Neighbors; } }

        public abstract System.Func<Node, Node, float> GetCostFunction { get; }
    }

    public class PositionData : WritableNeighborData
    {
        public Vector3 Position { get; set; }

        public override System.Func<Node, Node, float> GetCostFunction
        {
            get
            {
                return (a, b) => (a.Data as PositionData).Position.Distance(
                                 (b.Data as PositionData).Position);
            }
        }

        public PositionData(Vector3 position = new Vector3(), IEnumerable<Node> neighbors = null)
        {
            Position = position;

            if (neighbors != null)
                Neighbors.UnionWith(neighbors);
        }
    }

    public class Path : List<Node>
    {
        public Path(List<Node> nodes = null)
        {
            if(nodes != null)
                AddRange(nodes);
        }

        public Node GetNearestNode(Vector3 position)
        {
            return this.MinElement(node => (node.Data as PositionData).Position
                .Distance(position));
        }
    }
}


public class HighwaySystem
{
    Graph terrain_graph;
    Graph city_graph;

    Graph outgoing_roads;
    Graph incoming_roads;

    public HighwaySystem(Terrain terrain, IEnumerable<Vector3> cities)
    {
        terrain_graph = Graph.CreateGrid(terrain);
        city_graph = Graph.CreateHairball(cities).MinimumSpanned();

        outgoing_roads = terrain_graph.Defleshed(city_graph);
        incoming_roads = terrain_graph.Difference(outgoing_roads).WithoutHiddenNodes().Defleshed(city_graph);
        outgoing_roads = outgoing_roads.WithoutHiddenNodes();
    }

    public Graph.Path PlanRoadtrip(Vector3 source, Vector3 destination)
    {
        Graph.Path city_path = city_graph.GetNearestNode(source).GetPathTo(
                               city_graph.GetNearestNode(destination));

        Graph.Path road_path = new Graph.Path();
        for(int i = 0; i< (city_path.Count - 1); i++)
        {
            Vector3 source_city_position = (city_path[i].Data as Graph.PositionData).Position;
            Vector3 destination_city_position = (city_path[i + 1].Data as Graph.PositionData).Position;

            Graph roads;

            Vector3 displacement = destination_city_position - source_city_position;
            if (displacement.x * displacement.y < 0)
                roads = outgoing_roads;
            else
                roads = incoming_roads;

            Graph.Path leg = roads.GetNearestNode(source_city_position).GetPathTo(
                             roads.GetNearestNode(destination_city_position));

            if (road_path.Count == 0)
                road_path.Add(leg.First());
            leg.Remove(leg.First());

            foreach(Graph.Node node in leg)
                road_path.Add(node);
        }

        road_path.Add(new Graph.Node(new Graph.PositionData(destination)));

        return road_path;
    }
}


