using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Graph;

public class HighwaySystem
{
    Graph terrain_graph;
    Graph city_graph;

    public Graph OutgoingRoads { get; private set; }
    public Graph IncomingRoads { get; private set; }

    public float RoadWidth { get; set; } = 6;

    public HighwaySystem(Terrain terrain, IEnumerable<Vector3> cities)
    {
        terrain_graph = GraphUtility.CreateGrid(terrain);

        Vector3 terrain_size = terrain.terrainData.size;
        city_graph = GraphUtility.CreateHairball(cities)
            .MinimumSpanned()
            .WithNoLooseEnds();

        OutgoingRoads = terrain_graph.Defleshed(city_graph);

        IncomingRoads = terrain_graph
            .Difference(OutgoingRoads)
            .WithoutHiddenNodes()
            .Defleshed(city_graph)
            .WithoutHiddenNodes();

        OutgoingRoads = OutgoingRoads.WithoutHiddenNodes();
    }

    public Path PlanRoadtrip(Vector3 source, Vector3 destination)
    {
        Path city_path = city_graph.GetNearestNode(source).GetPathTo(
                               city_graph.GetNearestNode(destination));

        Path road_path = new Path();
        for (int i = 0; i < (city_path.Count - 1); i++)
        {
            Vector3 source_city_position = (city_path[i].Data as PositionData).Position;
            Vector3 destination_city_position = (city_path[i + 1].Data as PositionData).Position;

            Graph roads;

            Vector3 displacement = destination_city_position - source_city_position;
            if (displacement.x * displacement.y < 0)
                roads = OutgoingRoads;
            else
                roads = IncomingRoads;

            Path leg = roads.GetNearestNode(source_city_position).GetPathTo(
                             roads.GetNearestNode(destination_city_position));

            if (road_path.Count == 0)
                road_path.Add(leg.First());
            leg.Remove(leg.First());

            foreach (Node node in leg)
                road_path.Add(node);
        }

        road_path.Add(new Node(new PositionData(destination)));

        return road_path;
    }

    public bool IsObstructingTraffic(Vector3 position, float radius)
    {
        foreach (Edge edge in OutgoingRoads.Edges.Union(IncomingRoads.Edges))
        {
            LineSegment road = new LineSegment(edge.A.GetPosition(), edge.B.GetPosition());

            if (position.Distance(road) < (radius + RoadWidth))
                return true;
        }

        return false;
    }
}