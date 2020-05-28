using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Graph;

public static class GraphUtility
{
    public static Graph CreateGrid(float width, float height, float resolution,
                                 Vector3 offset = new Vector3())
    {
        Graph graph = new Graph();

        int column_count = (int)(width / resolution);
        int row_count = (int)(height / resolution);

        Node[,] grid = new Node[column_count, row_count];

        for (int x = 0; x < column_count; x++)
        {
            float normalized_x = x / (column_count - 1.0f);

            for (int y = 0; y < row_count; y++)
            {
                float normalized_y = y / (row_count - 1.0f);

                graph.AddNode(grid[x, y] =
                    new Node(new PositionData(
                        new Vector3(width * normalized_x,
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

        for (int x = 0; x < column_count; x++)
        {
            for (int y = 0; y < row_count; y++)
            {
                Node node = grid[x, y];

                foreach (Vector2Int displacement in neighbor_displacements)
                {
                    Vector2Int index = new Vector2Int(x, y) + displacement;

                    if (index.x >= 0 && index.x < column_count &&
                        index.y >= 0 && index.y < row_count)
                        node.AddNeighbor(grid[index.x, index.y]);
                }
            }
        }

        return graph;
    }

    public static Graph CreateGrid(Terrain terrain, float resolution)
    {
        float width = terrain.terrainData.size.x;
        float height = terrain.terrainData.size.z;

        Graph grid = CreateGrid(width, height, resolution, terrain.GetPosition());

        foreach (Node node in grid.Nodes)
            node.SetPosition(node.GetPosition().YChangedTo(terrain.SampleHeight(node.GetPosition())));

        return grid;
    }

    public static Graph CreateHairball<T>(IEnumerable<T> position_datas) where T : PositionData
    {
        List<Node> nodes = new List<Node>();

        foreach (PositionData position_data in position_datas)
            nodes.Add(new Node(position_data));

        foreach (Node node in nodes)
            foreach (Node other_node in nodes)
                if (node != other_node)
                    node.AddNeighbor(other_node);

        return new Graph(nodes);
    }

    public static Graph CreateHairball(IEnumerable<Vector3> positions)
    {
        return CreateHairball(positions.Select(position => new PositionData(position)));
    }

    //"Blocks" as in city blocks. It finds networks of nodes 
    //by comparing distance between nodes to max_neighbor_distance
    public static List<Graph> GetBlocks(IEnumerable<PositionData> position_datas,
                                        float max_neighbor_distance)
    {
        List<Graph> blocks = new List<Graph>();

        HashSet<Node> all_nodes = new HashSet<Node>(position_datas
            .Select(position_data => new Node(position_data)));

        foreach (Node node in all_nodes)
            foreach (Node other_node in all_nodes)
                if (other_node != node)
                {
                    float max_distance = max_neighbor_distance +
                                         node.GetSize() +
                                         other_node.GetSize();

                    float distance = node.GetPosition().Distance(other_node.GetPosition());

                    if (node.GetPosition().Distance(other_node.GetPosition()) < max_distance)
                        node.AddNeighbor(other_node);
                }

        while (all_nodes.Count > 0)
        {
            Node node = all_nodes.First();

            Graph block = new Graph(node.GetConnectedNodes());
            foreach (Node block_node in block.Nodes)
                all_nodes.Remove(block_node);

            blocks.Add(block);
        }

        return blocks;
    }

    public static Graph CreateGraph(IEnumerable<Edge> edges, bool remove_hidden_edges = true)
    {
        if (!remove_hidden_edges)
        {
            HashSet<Node> nodes = new HashSet<Node>();
            foreach (Edge edge in edges)
            {
                nodes.Add(edge.A);
                nodes.Add(edge.B);
            }

            return new Graph(nodes);
        }


        Dictionary<Node, Node> node_map = new Dictionary<Node, Node>();

        foreach (Edge edge in new HashSet<Edge>(edges))
        {
            if (!node_map.ContainsKey(edge.A))
                node_map[edge.A] = edge.A.Copy();

            if (!node_map.ContainsKey(edge.B))
                node_map[edge.B] = edge.B.Copy();

            node_map[edge.A].AddNeighbor(node_map[edge.B]);
        }

        return new Graph(node_map.Values);
    }

    public static Graph CreateGraph(IEnumerable<Path> paths, bool remove_hidden_edges = true)
    {
        List<Edge> edges = new List<Edge>();

        foreach (Path path in paths)
            for (int i = 0; i < path.Count() - 1; i++)
                edges.Add(new Edge(path[i], path[i + 1]));

        return CreateGraph(edges, remove_hidden_edges);
    }


    public static Node CreatePositionNode(Vector3 position)
    {
        return new Node(new PositionData(position));
    }


    public delegate float Metric(Node a, Node b);

    public static Dictionary<Metric, Metric> Heuristics { get; private set; } =
        new Dictionary<Metric, Metric>();


    public static Metric EuclideanMetric { get; private set; }
    public static Metric ObstacleMetric { get; private set; }

    public static Metric CreateHeightMetric(System.Func<Vector3, float> get_height,
                                            System.Func<float, float> get_height_cost,
                                            System.Func<float, float> get_slope_cost,
                                            float resolution)
    {
        System.Func<Vector3, Vector3, float> get_slope =
            (a, b) => (get_height(b) - get_height(a)) / a.Distance(b);

        return delegate (Node a, Node b)
        {
            Vector3 displacement = b.GetPosition() - a.GetPosition();

            int segment_count = (int)(0.5f + displacement.magnitude / resolution);

            float cost_sum = 0;
            for (int i = 0; i < segment_count; i++)
            {
                Vector3 p0 = a.GetPosition() + displacement * i / segment_count;
                Vector3 p1 = a.GetPosition() + displacement * (i + 1) / segment_count;

                float p0_height = get_height(p0);
                float p1_height = get_height(p1);
                float distance = p0.YChangedTo(p0_height).Distance(
                                    p1.YChangedTo(p1_height));

                float height_cost = (get_height_cost(p0_height) + get_height_cost(p1_height)) / 2;
                float slope_cost = get_slope_cost((p1_height - p0_height) / distance);

                cost_sum += (height_cost + slope_cost) * distance;
            }

            return cost_sum;
        };
    }

    public static Metric CreateBakedMetric(Metric metric)
    {
        return delegate (Node a, Node b)
        {
            Edge edge = new Edge(a, b);

            if (!metric_table.ContainsKey(metric))
                metric_table[metric] = new Dictionary<Edge, float>();

            if (!metric_table[metric].ContainsKey(edge))
                metric_table[metric][edge] = metric(a, b);

            return metric_table[metric][edge];
        };
    }


    static Dictionary<Metric, Dictionary<Edge, float>> metric_table =
        new Dictionary<Metric, Dictionary<Edge, float>>();

    static GraphUtility()
    {
        EuclideanMetric = CreateBakedMetric((a, b) => a.GetPosition().Distance(b.GetPosition()));
        Heuristics[EuclideanMetric] = EuclideanMetric;

        //This metric is directional, the cost from a to b
        //is not necessarily the same as from b to a
        ObstacleMetric = CreateBakedMetric(delegate (Node a, Node b)
        {
            float height_cost = CreateHeightMetric(
                position => Scene.Main.World.Asteroid.GetSurfaceHeight(position),
                height => 0,
                slope => Mathf.Pow((Mathf.Max(0, slope) / 0.3f), 2),
                HighwaySystem.TerrainGridResolution * Mathf.Sqrt(2))(a, b);

            return height_cost + EuclideanMetric(a, b);
        });
        Heuristics[ObstacleMetric] = EuclideanMetric;
    }

    public static void ClearMetricTable() { metric_table.Clear(); }
}