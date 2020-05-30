using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Graph;

public class HighwaySystem
{
    public Graph Highways { get; set; }

    public float RoadWidth { get; set; } = 6;
    public float OpposingTrafficMargin { get { return RoadWidth * 2; } }

    public HighwaySystem(Terrain terrain, IEnumerable<Vector3> cities)
    {
        Vector3 terrain_size = terrain.terrainData.size;
        Vector3 terrain_center = terrain.GetPosition() + terrain_size / 2;

        Graph terrain_graph = GraphUtility.CreateGrid(terrain, TerrainGridResolution);
        Graph city_graph = GraphUtility.CreateHairball(cities).MinimumSpanned_Obstacle();


        //Select capital
        Node capital = city_graph.Nodes
            .Where(node => node.Neighbors.Count() == 1)
            .RandomElement();

        //Get edges leaving the capital
        HashSet<Edge> left_edges = new HashSet<Edge>();
        foreach (Node node in city_graph.Nodes)
            if (node != capital)
                left_edges.UnionWith(capital.GetPathTo_Euclidean(node).Edges);
        
        //Embed left_city_graph within terrain_graph
        Graph left_city_graph = GraphUtility.CreateGraph(left_edges);
        Graph left_lanes = terrain_graph.Defleshed(left_city_graph, 
                                                   GraphUtility.ObstacleMetric);

        //Do the same for right_city_graph, but alter the cost metric
        //so the right_lanes prefer a margin to exist between
        //opposing lanes of traffic. 
        Graph right_city_graph = GraphUtility.CreateGraph(
            left_edges.Select(edge => new Edge(edge.B, edge.A)));
        foreach (Node node in right_city_graph.Nodes)
        {
            Edge edge;

            if (node.GetPosition() == capital.GetPosition())
                edge = right_city_graph.Edges.FirstOrDefault(edge_ => edge_.B == node);
            else
                edge = right_city_graph.Edges.FirstOrDefault(edge_ => edge_.A == node);

            Vector3 offset = (edge.A.GetPosition() - edge.B.GetPosition())
                .Crossed(new Vector3(0, 1, 0)).normalized * OpposingTrafficMargin;

            node.SetPosition(node.GetPosition() + offset);
        }

        GraphUtility.Metric opposing_traffic_metric = GraphUtility.CreateBakedMetric(
        delegate (Node a, Node b)
        {
            System.Func<Vector3, float> get_height =
                position => Mathf.Max(0,
                    (OpposingTrafficMargin - left_lanes.Distance(position)) / 
                    OpposingTrafficMargin);

            return GraphUtility.CreateHeightMetric(
                        get_height,
                        height => height * 2,
                        slope => 0,
                        TerrainGridResolution * Mathf.Sqrt(2))(a, b) +

                   GraphUtility.ObstacleMetric(a, b); ;
        });
        GraphUtility.Heuristics[opposing_traffic_metric] = GraphUtility.EuclideanMetric;

        Graph right_lanes = terrain_graph
            .Defleshed(right_city_graph, opposing_traffic_metric);

        //Merge duplicate nodes
        foreach (Node node in left_lanes.Nodes.ToList())
            foreach (Node other_node in right_lanes.Nodes)
            {
                if (node.GetPosition() != other_node.GetPosition())
                    continue;

                left_lanes.RemoveNode(node);
                left_lanes.AddNode(other_node);

                foreach (Node neighbor in node.Neighbors)
                    other_node.AddNeighbor(neighbor);

                IEnumerable<Node> neighbors = left_lanes.Nodes
                    .Where(potential_neighbor => potential_neighbor.Neighbors.Contains(node));
                foreach (Node neighbor in neighbors.ToList())
                {
                    neighbor.RemoveNeighbor(node);
                    neighbor.AddNeighbor(other_node);
                }
            }

        //Create "bridges" from one highway system to the other.
        int bridge_count = 0;
        bool is_forward_edge = true;
        foreach (Node node in left_lanes.Nodes)
        {
            if ((bridge_count++ % 4) == 0)
            {
                Node other_node = right_lanes.GetNearestNode(
                    other_node_ => is_forward_edge ? GraphUtility.ObstacleMetric(node, other_node_) : 
                                                     GraphUtility.ObstacleMetric(other_node_, node),
                    
                    other_node_ => node != other_node_ &&
                                   !node.Neighbors.Contains(other_node_) &&
                                   !other_node_.Neighbors.Contains(node));

                if (is_forward_edge)
                    node.AddNeighbor(other_node);
                else
                    other_node.AddNeighbor(node);

                is_forward_edge = !is_forward_edge;
            }
        }

        Highways = GraphUtility.CreateGraph(
            left_lanes.Edges.Union(right_lanes.Edges));

        //Fully connect the endpoints of paths because their connectivity
        //tends to be problematic. 
        HashSet<Node> endpoints = new HashSet<Node>(
            left_city_graph.Nodes.Union(right_city_graph.Nodes)
            .Select(node => Highways.GetNearestNode(node)));

        foreach (Node node in endpoints)
            foreach (Node other_node in Highways.Nodes)
                if (node != other_node && 
                    node.Distance(other_node) < 1.2f * Mathf.Sqrt(2) * TerrainGridResolution)
                    node.AddNeighbor(other_node);
    }

    public Path PlanRoadtrip(Vector3 source, Vector3 destination)
    {
        Node source_node = GraphUtility.CreatePositionNode(source).AsGuestTo(Highways);
        Node destination_node = GraphUtility.CreatePositionNode(destination);
        Path roadtrip = source_node.GetPathTo_Obstacle(destination_node);

        if (roadtrip == null)
            return null;

        return roadtrip;
    }

    public bool IsObstructingTraffic(Vector3 position, float radius)
    {
        foreach (Edge edge in Highways.Edges)
        {
            LineSegment road = new LineSegment(edge.A.GetPosition(), edge.B.GetPosition());

            if (position.Distance(road) < (radius + RoadWidth))
                return true;
        }

        return false;
    }


    public static float TerrainGridResolution { get; set; } = 10;
}