using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Graph;

public static class GraphUtility
{
    public static Graph CreateGrid(float width, float height, int resolution,
                                 Vector3 offset = new Vector3())
    {
        Graph graph = new Graph();

        Node[,] grid = new Node[resolution, resolution];

        for (int x = 0; x < resolution; x++)
        {
            float normalized_x = x / (resolution - 1.0f);

            for (int y = 0; y < resolution; y++)
            {
                float normalized_y = y / (resolution - 1.0f);

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

        for (int x = 0; x < resolution; x++)
        {
            for (int y = 0; y < resolution; y++)
            {
                Node node = grid[x, y];

                foreach (Vector2Int displacement in neighbor_displacements)
                {
                    Vector2Int index = new Vector2Int(x, y) + displacement;

                    if (index.x >= 0 && index.x < resolution &&
                        index.y >= 0 && index.y < resolution)
                        node.AddNeighbor(grid[index.x, index.y]);
                }
            }
        }

        return graph;
    }

    public static Graph CreateGrid(Terrain terrain)
    {
        float width = terrain.terrainData.size.x;
        float height = terrain.terrainData.size.z;

        Graph grid = CreateGrid(width, height, (int)(Mathf.Max(width, height) / 5), terrain.GetPosition());

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

            Graph block = node.GetConnectedNodes();
            foreach (Node block_node in block.Nodes)
                all_nodes.Remove(block_node);

            blocks.Add(block);
        }

        return blocks;
    }
}