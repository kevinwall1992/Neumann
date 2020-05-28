using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;
using static Graph;

public static class GraphExtensions
{
    public static Graph Defleshed(this Graph graph, Graph xray)
    {
        Dictionary<Node, Node> xray_analogs = new Dictionary<Node, Node>();

        foreach (Node node in xray.Nodes)
            xray_analogs[node] = graph.GetNearestNode(node);

        HashSet<Node> skeleton = new HashSet<Node>();

        foreach (Edge edge in xray.Edges)
            foreach (Node path_node in xray_analogs[edge.A].GetPathTo(xray_analogs[edge.B]))
                skeleton.Add(path_node);

        return new Graph(skeleton);
    }

    public static Graph Difference(this Graph graph, Graph other)
    {
        Graph difference = new Graph(graph.Nodes);

        foreach (Node node in graph.Nodes)
            if (other.Nodes.Contains(node))
                difference.RemoveNode(node);

        return difference;
    }

    public static Graph Union(this Graph graph, Graph other)
    {
        Graph union = new Graph(graph.Nodes);

        foreach (Node node in other.Nodes)
            if (!union.Nodes.Contains(node))
                union.AddNode(node);

        return union;
    }

    //Requires PositionData
    public static Graph MinimumSpanned(this Graph graph)
    {
        if (graph.Nodes.Count() == 0)
            return new Graph();

        HashSet<Node> open_set = new HashSet<Node>(graph.WithoutHiddenNodes().Nodes);

        Graph mst = new Graph();
        mst.AddNode(open_set.First());
        open_set.Remove(open_set.First());

        while (open_set.Count > 0)
        {
            Edge minimum_edge = mst.Edges
                .Where(edge => open_set.Contains(edge.B))
                .MinElement(edge => edge.A.Cost(edge.B));

            Node node = minimum_edge.B;

            foreach (Node neighbor in new List<Node>(node.Neighbors))
                if (mst.Nodes.Contains(neighbor) && neighbor != minimum_edge.A)
                    node.RemoveNeighbor(neighbor);

            open_set.Remove(node);
            mst.AddNode(node);

            foreach (Node tree_node in mst.Nodes)
                if (tree_node != minimum_edge.A && tree_node.Neighbors.Contains(node))
                    tree_node.RemoveNeighbor(node);
        }

        return mst;
    }

    //Requires PositionData
    public static Graph WithNoLooseEnds(this Graph graph)
    {
        Graph no_loose_ends = new Graph(graph.WithoutHiddenNodes().Nodes);

        foreach (Node node in no_loose_ends.Nodes)
        {
            if (node.Neighbors.Count() == 1)
            {
                Node existing_neighbor = node.Neighbors.First();

                Vector3 existing_neighbor_direction =
                    (existing_neighbor.GetPosition() - node.GetPosition()).normalized;

                Node new_neighbor = no_loose_ends.Nodes.MinElement(other_node =>

                    (other_node != node &&
                     other_node != existing_neighbor &&
                     other_node.IsConnectedTo(node)) ?

                    other_node.GetPosition().Distance(node.GetPosition()) *
                    existing_neighbor_direction.Dot((other_node.GetPosition() - node.GetPosition())) :

                    float.MaxValue);

                if (new_neighbor != null)
                {
                    node.AddNeighbor(new_neighbor);
                    new_neighbor.AddNeighbor(node);
                }
            }
        }

        return no_loose_ends;
    }

    //Prunes edges where
    //  1) destination node is neighbor to two nodes who are 
    //     themselves neighbors (a triangle)
    //  2) edge between neighbors is shorter
    //  3) angle between edge and edge between neighbors is less
    //     than minimum_angle

    //At a high level, gets rid of long, thin triangles in graphs
    //by removing the longest edge. 
    public static Graph EdgesPrunedByAngle(this Graph graph, float minimum_angle)
    {
        Graph pruned = new Graph();

        HashSet<Node> open_set = new HashSet<Node>(graph.WithoutHiddenNodes().Nodes);

        while (open_set.Count > 0)
        {
            Node node = open_set.First();
            open_set.Remove(node);
            pruned.AddNode(node);

            HashSet<Node> neighbors_set = new HashSet<Node>(node.Neighbors);

            while (neighbors_set.Count > 0)
            {
                Node neighbor = neighbors_set.MinElement(neighbor_ =>
                    neighbor_.GetPosition().Distance(node.GetPosition()));
                neighbors_set.Remove(neighbor);

                Vector3 node_to_neighbor = neighbor.GetPosition() - node.GetPosition();

                IEnumerable<Node> shared_neighbors = neighbor.Neighbors
                    .Where(neighbors_neighbor => node.Neighbors.Contains(neighbors_neighbor));

                foreach (Node shared_neighbor in shared_neighbors)
                {
                    Vector3 node_to_shared_neighbor = shared_neighbor.GetPosition() -
                                                      node.GetPosition();

                    if (node_to_neighbor.AngleBetween(node_to_shared_neighbor) < minimum_angle)
                    {
                        node.RemoveNeighbor(shared_neighbor);
                        shared_neighbor.RemoveNeighbor(node);

                        neighbors_set.Remove(shared_neighbor);
                    }
                }
            }
        }

        return pruned;
    }

    public static Graph WithoutHiddenNodes(this Graph graph)
    {
        return graph.Copy(true);
    }

    //Only use remove_hidden_neighbors option if 
    //neighbors are writable
    public static Graph Copy(this Graph graph, bool remove_hidden_nodes = false)
    {
        HashSet<Node> nodes;

        if (!remove_hidden_nodes)
        {
            nodes = new HashSet<Node>();

            HashSet<Node> open_set = new HashSet<Node>();
            open_set.UnionWith(graph.Nodes);

            while (open_set.Count > 0)
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
            nodes = new HashSet<Node>(graph.Nodes);


        Graph copy = new Graph();

        Dictionary<Node, Node> copy_map = new Dictionary<Node, Node>();

        foreach (Node node in nodes)
        {
            Node node_copy = null;

            if (node.Data is UnitData)
                node_copy = new Node(new UnitData(node.GetUnit()));
            else if (node.Data is PositionData)
                node_copy = new Node(new PositionData(node.GetPosition()));
            else
                Debug.Assert(false, "Can't copy node, has unsupported IData type.");

            copy.AddNode(node_copy);
            copy_map[node] = node_copy;
        }

        Dictionary<Node, Node> original_map = copy_map.Inverted();

        foreach (Node node in copy.Nodes)
            foreach (Node neighbor in original_map[node].Neighbors)
                if (copy_map.ContainsKey(neighbor))
                    node.AddNeighbor(copy_map[neighbor]);

        return copy;
    }

    public static Sphere GetBoundingSphere(this Graph graph)
    {
        return MathUtility.GetBoundingSphere(graph.Nodes
            .Where(node => node.HasPosition())
            .Select(node => new Sphere(node.GetPosition(), node.GetSize())));
    }

    public static Node GetNearestNode(this Graph graph, System.Func<Node, float> metric)
    {
        return graph.Nodes.MinElement(node => metric(node));
    }

    public static Node GetNearestNode(this Graph graph, Node node)
    {
        return graph.GetNearestNode(node_ => node_.Cost(node));
    }

    public static Node GetNearestNode(this Graph graph, Vector3 position)
    {
        return graph.GetNearestNode(node => node.GetPosition().Distance(position));
    }
}


public static class NodeExtensions
{
    //Assumes admissible, consistant heuristic
    public static Path GetPathTo(this Node source, Node destination)
    {
        bool destination_is_guest = !source.IsConnectedTo(destination);

        Dictionary<Node, Node> previous_node = new Dictionary<Node, Node>();
        Dictionary<Node, float> cost_to_source = new Dictionary<Node, float>();
        Dictionary<Node, float> cost_to_destination_estimate = new Dictionary<Node, float>();

        System.Func<Node, float> GetPathCostEstimate =
            node => cost_to_source[node] + cost_to_destination_estimate[node];

        SortedSet<Node> open_set = new SortedSet<Node>(
            Comparer<Node>.Create((a, b) => GetPathCostEstimate(a).CompareTo(GetPathCostEstimate(b))));
        HashSet<Node> closed_set = new HashSet<Node>();

        open_set.Add(source);
        previous_node[source] = null;
        cost_to_source[source] = 0;
        cost_to_destination_estimate[source] = 0;

        while (open_set.Count > 0)
        {
            Node node = open_set.First();
            open_set.Remove(node);
            closed_set.Add(node);

            if (node == destination)
                break;

            IEnumerable<Node> neighbors = node.Neighbors;
            if (destination_is_guest)
                neighbors = neighbors.Union(Utility.List(destination));

            foreach (Node neighbor in neighbors)
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

    public static Graph GetConnectedNodes(this Node node)
    {
        Graph graph = new Graph();

        HashSet<Node> open_set = new HashSet<Node>();
        open_set.Add(node);

        while (open_set.Count > 0)
        {
            Node connected_node = open_set.First();
            open_set.Remove(connected_node);
            graph.AddNode(connected_node);

            foreach (Node neighbor in connected_node.Neighbors)
                if (!graph.Nodes.Contains(neighbor))
                    open_set.Add(neighbor);
        }

        return graph;
    }

    public static bool IsConnectedTo(this Node node, Node other)
    {
        return node.GetConnectedNodes().Nodes.Contains(other);
    }

    public static bool AreNeighborsWritable(this Node node)
    {
        return node.Data is WritableNeighborData;
    }

    public static Node AddNeighbor(this Node node, Node neighbor)
    {
        if (node.AreNeighborsWritable())
            (node.Data as WritableNeighborData).Neighbors.Add(neighbor);

        return node;
    }

    public static Node RemoveNeighbor(this Node node, Node neighbor)
    {
        if (node.AreNeighborsWritable())
            (node.Data as WritableNeighborData).Neighbors.Remove(neighbor);

        return node;
    }

    public static bool HasPosition(this Node node)
    {
        return node.Data is PositionData;
    }

    public static Vector3 GetPosition(this Node node)
    {
        if (node.HasPosition())
            return (node.Data as PositionData).Position;

        return Vector3.zero;
    }

    public static void SetPosition(this Node node, Vector3 position)
    {
        if (node.HasPosition())
            (node.Data as PositionData).Position = position;
    }

    public static float GetSize(this Node node)
    {
        if (node.Data is SphereData)
            return (node.Data as SphereData).Radius;

        return 0;
    }

    public static bool HasUnit(this Node node)
    {
        return node.Data is UnitData;
    }

    public static Unit GetUnit(this Node node)
    {
        if (node.HasUnit())
            return (node.Data as UnitData).Unit;

        return null;
    }
}