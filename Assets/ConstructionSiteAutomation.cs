using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using static Graph;

public static class ConstructionSiteAutomation
{
    static float MaxBlockNeighborDistance { get { return 1; } }
    static float BlockMargin { get { return MaxBlockNeighborDistance + 6; } }
    static float MaxBlockConnectivity { get { return 3.2f; } }

    public static void ValidateConstructionSite(this BuildTask this_build_task, Builder builder)
    {
        //Find building complex our BuildTask is a part of
        List<BuildTask> build_task_segment = builder.Unit.Program
            .GetSegment(this_build_task)
            .Where(operation => operation is BuildTask &&
                                !((operation as BuildTask).Blueprint.HasComponent<Motile>()))
            .Select(operation => operation as BuildTask)
            .ToList();

        int build_task_index = build_task_segment.IndexOf(this_build_task);
        List<BuildTask> complex =
            build_task_segment.GetRange(build_task_index,
                                        build_task_segment.Count - build_task_index);

        //Determine if building complex is blocked
        bool complex_is_blocked = false;
        foreach (BuildTask build_task in complex)
            if (!build_task.IsConstructionSiteClear)
                complex_is_blocked = true;

        if (!complex_is_blocked)
            return;


        Sphere complex_bounding_sphere = MathUtility.GetBoundingSphere(complex
            .Select(build_task => new Sphere(build_task.ConstructionSite,
                                                build_task.ConstructionSiteSize)));
        Vector3 complex_center = complex_bounding_sphere.Point;
        float complex_radius = complex_bounding_sphere.Radius;

        //Get block structure and nearby buildings within those blocks.
        //A "block" is a collection buildings separated by space (i.e. roads)

        List<Graph> blocks = GraphUtility.GetBlocks(
            Scene.Main.World.Buildings
                .Select(building => new UnitData(building)),
            MaxBlockNeighborDistance);

        HashSet<Node> nearby_building_nodes = new HashSet<Node>();
        foreach (Graph block in blocks)
            if (block.Nodes.FirstOrDefault(node =>
                node.GetPosition().Distance(complex_center) < complex_radius * 8) != null)
                nearby_building_nodes.UnionWith(block.Nodes);


        //Finds all blocks that would be effected by a candidate construction site. 
        //"Effected" here means they would aquire new nodes because we would 
        //be building new buildings on that block.

        Dictionary<Vector3, Dictionary<Graph, Dictionary<BuildTask, int>>> blocks_abutted_map =
            new Dictionary<Vector3, Dictionary<Graph, Dictionary<BuildTask, int>>>();

        System.Func<Vector3, Dictionary<Graph, Dictionary<BuildTask, int>>> GetBlocksAbutted =
        delegate (Vector3 candidate_site)
        {
            if (!blocks_abutted_map.ContainsKey(candidate_site))
            {
                Vector3 displacement = candidate_site - complex_center;

                Dictionary<Graph, Dictionary<BuildTask, int>> blocks_abutted =
                    new Dictionary<Graph, Dictionary<BuildTask, int>>();

                foreach (BuildTask build_task in complex)
                {
                    Vector3 new_construction_site = build_task.ConstructionSite + displacement;

                    foreach (Graph block in blocks)
                        foreach (Node node in block.Nodes)
                            if (new_construction_site.Distance(node.GetPosition()) <
                                (build_task.ConstructionSiteSize +
                                node.GetSize() +
                                BlockMargin))
                            {
                                if (!blocks_abutted.ContainsKey(block))
                                    blocks_abutted[block] = new Dictionary<BuildTask, int>();

                                if (!blocks_abutted[block].ContainsKey(build_task))
                                    blocks_abutted[block][build_task] = 0;

                                blocks_abutted[block][build_task]++;
                            }
                }

                blocks_abutted_map[candidate_site] = blocks_abutted;
            }

            return blocks_abutted_map[candidate_site];
        };


        //Estimating the connectivity that would result
        //from a candidate construction site. Assumes candidate would
        //only abut one block, and that that the resultant connectivity
        //would only depend on the buildings that directly abut the block
        //(Ignoring new buildings that abut the abutting buildings)

        Dictionary<Vector3, float> resultant_connectivity_map = new Dictionary<Vector3, float>();

        System.Func<Vector3, float> EstimateResultantConnectivity = delegate (Vector3 candidate_site)
        {
            if (!resultant_connectivity_map.ContainsKey(candidate_site))
            {
                Dictionary<Graph, Dictionary<BuildTask, int>> blocks_abutted =
                    GetBlocksAbutted(candidate_site);

                //We assume blocks abutted count > 0, because otherwise, we shouldn't
                //be collided in the first place, right? If this assumption does not
                //hold, we need some sort of special handling because such a 
                //candidate will not have a connectivity metric. 
                Debug.Assert(blocks_abutted.Keys.Count() > 0,
                                    "candidate site does not abut any existing blocks");

                Graph abutting_block = blocks_abutted.Keys.First();

                int new_edges = 0;
                int new_nodes = 0;

                foreach (BuildTask build_task in blocks_abutted[abutting_block].Keys)
                {
                    new_nodes++;

                    new_edges += abutting_block.Nodes
                        .Where(node => node.GetPosition().Distance(candidate_site) <
                                        (MaxBlockNeighborDistance +
                                        node.GetSize() +
                                        complex_radius)).Count() * 2;
                }

                resultant_connectivity_map[candidate_site] =
                    (abutting_block.Connectivity * abutting_block.Nodes.Count() + new_edges) /
                    (abutting_block.Nodes.Count() + new_nodes);
            }

            return resultant_connectivity_map[candidate_site];
        };


        //Culls candidate construction sites based on a set of criteria, 
        //such as collisions with existing features and topological 
        //concerns with regards to transport. 

        System.Func<Vector3, bool> IsCandidateSiteValid = delegate (Vector3 candidate_site)
        {
            //Not within terrain boundaries
            Terrain terrain = Scene.Main.World.Asteroid.Terrain;
            Vector2 normalized_xz = (candidate_site.XZ() - terrain.GetPosition().XZ()) /
                                    terrain.terrainData.size.XZ();
            if (normalized_xz.x < 0 || normalized_xz.x > 1 ||
                normalized_xz.y < 0 || normalized_xz.y > 1)
                return false;


            //Too steep
            float slope_in_degrees = terrain.terrainData
                .GetSteepness(normalized_xz.x, normalized_xz.y);
            if (slope_in_degrees != 0)
                if (slope_in_degrees > 20)
                    return false;


            //Must be on the frontier of the abutting block. 
            //This is to prevent winding and branching
            Sphere block_bounding_sphere = GetBlocksAbutted(candidate_site).Keys.First()
                .GetBoundingSphere();
            float distance_to_center = candidate_site.Distance(block_bounding_sphere.Point);
            if ((block_bounding_sphere.Radius - distance_to_center) > (complex_radius + 1.2f))
                return false;


            //Collision with existing buildings
            foreach (Unit unit in Scene.Main.World.Buildings)
                if (unit.Physical.Position.Distance(candidate_site) <
                    (unit.Physical.Size + complex_radius))
                    return false;


            //Collision with highways
            if (Scene.Main.World.Asteroid.HighwaySystem
                .IsObstructingTraffic(candidate_site, complex_radius))
                return false;


            //Candidate might connect separate blocks together
            Dictionary<Graph, Dictionary<BuildTask, int>> blocks_abutted =
                GetBlocksAbutted(candidate_site);
            if (blocks_abutted.Keys.Count() > 1)
                return false;


            //Would this increase connectivity to undesirable levels?
            //(Higher connectivity means lower surface area,
            //thus buildings may not be exposed to traffic)
            float connectivity = blocks_abutted.Keys.First().Connectivity;
            float resultant_connectivity = EstimateResultantConnectivity(candidate_site);

            if (connectivity >= MaxBlockConnectivity)
            {
                if (resultant_connectivity >= connectivity)
                    return false;
            }
            else if (resultant_connectivity > MaxBlockConnectivity)
                return false;


            return true;
        };


        //Generate candidate construction sites by placing the building
        //complex center such that it "cuddles" two existing buildings
        //that neighbor one another. 

        //      #######         #######
        //    #         #     #         #
        //   # Building  #   # Building  #  
        //   #     A     #   #     B     #  
        //    #         #     #         #
        //      #######         #######     
        //          >   #######   <  
        //         /  #         #  \ 
        //        /  #  Complex  #  \
        //       /   #           #   \
        //      /     #         #     \
        // Cuddling     #######    Cuddling

        System.Func<List<Vector3>> GenerateCandidateSites_Cuddle = delegate ()
        {
            List<Vector3> candidate_sites_ = new List<Vector3>();

            IEnumerable<Edge> edges = new Graph(nearby_building_nodes).Edges;

            foreach (Edge edge in edges)
            {
                Node a = edge.A,
                            b = edge.B;

                if (b.GetUnit().Physical.Position.Distance(complex_center) <
                    a.GetUnit().Physical.Position.Distance(complex_center))
                    Utility.Swap(ref a, ref b);

                Vector3 displacement = b.GetUnit().Physical.Position - a.GetUnit().Physical.Position;
                float space_between = displacement.magnitude - (a.GetSize() + b.GetSize());

                if (complex_radius * 2 > space_between &&
                    blocks.Find(block => block.Nodes.Contains(a)).Nodes.Contains(b))
                {
                    float a_side = complex_radius + b.GetSize(),
                            b_side = complex_radius + a.GetSize();

                    float a_angle = Mathf.Acos((Mathf.Pow(b_side, 2) +
                                                Mathf.Pow(displacement.magnitude, 2) -
                                                Mathf.Pow(a_side, 2)) /
                                                (2 * b_side * displacement.magnitude));

                    Vector3 direction = displacement.YChangedTo(0).normalized;
                    Vector3 perpendicular_direction = direction.Crossed(new Vector3(0, 1, 0));

                    float direction_length = Mathf.Cos(a_angle) * b_side;
                    float perpendicular_direction_length = Mathf.Sin(a_angle) * b_side * 1.01f;
                    float height = Mathf.Lerp(a.GetPosition().y,
                                                b.GetPosition().y,
                                                direction_length / displacement.magnitude);

                    candidate_sites_.Add(a.GetPosition().YChangedTo(height) +
                                        direction * direction_length +
                                        perpendicular_direction * perpendicular_direction_length);

                    candidate_sites_.Add(a.GetPosition().YChangedTo(height) +
                                        direction * direction_length -
                                        perpendicular_direction * perpendicular_direction_length);


                }
            }

            return candidate_sites_;
        };


        //Generates candidate construction sites by computing the minimum 
        //spanning tree of all nearby buildings and then attempts to place
        //complex between two neighboring buildings, as near as possible to the 
        //building nearest to the original building complex center.  

        System.Func<List<Vector3>> GenerateCandidateSites_Between = delegate ()
        {
            List<Vector3> candidate_sites_ = new List<Vector3>();

            Graph mst = GraphUtility.CreateHairball(nearby_building_nodes.Select(node =>
                    new UnitData(node.GetUnit())))
                .MinimumSpanned_Euclidean();

            foreach (Edge edge in mst.Edges)
            {
                Node a = edge.A,
                            b = edge.B;

                if (b.GetUnit().Physical.Position.Distance(complex_center) <
                    a.GetUnit().Physical.Position.Distance(complex_center))
                    Utility.Swap(ref a, ref b);

                Vector3 displacement = b.GetUnit().Physical.Position - a.GetUnit().Physical.Position;
                float space_between = displacement.magnitude - (a.GetSize() + b.GetSize());

                if (complex_radius * 2 <= space_between &&
                    blocks.Find(block => block.Nodes.Contains(a)).Nodes.Contains(b))
                    candidate_sites_.Add(complex_center +
                                            displacement.normalized *
                                            (a.GetSize() + complex_radius) *
                                            1.01f);
            }

            return candidate_sites_;
        };


        //Generates candidate construction sites by finding nearest building to
        //the original building complex center and then placing complex next to 
        //that building in n equally spaced radial directions. 

        System.Func<List<Vector3>> GenerateCandidateSites_Radial = delegate ()
        {
            List<Vector3> candidate_sites_ = new List<Vector3>();

            Unit nearest_building = nearby_building_nodes
                .MinElement(node => node.GetPosition().Distance(complex_center))
                .GetUnit();

            for (int i = 0; i < 10; i++)
            {
                float angle = (i / 10.0f) * (2 * Mathf.PI);

                candidate_sites_.Add(nearest_building.Physical.Position +
                                    new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) *
                                    (nearest_building.Physical.Size + complex_radius) * 1.01f);
            }

            return candidate_sites_;
        };


        //Attempt to find valid candidate construction sites using each
        //of the generation functions.

        //  1) Generate candidate sites using next generation function
        //  2) Cull invalid sites
        //  3a) If no sites remain, return to step 1, unless
        //      3b) No generation functions remain. BuildTask is canceled.

        List<Vector3> candidate_sites = null;

        List<System.Func<List<Vector3>>> candidate_site_generators =
            Utility.List(GenerateCandidateSites_Cuddle,
                            GenerateCandidateSites_Radial);

        foreach (System.Func<List<Vector3>> candidate_site_generator in candidate_site_generators)
        {
            candidate_sites = candidate_site_generator();

            foreach (Vector3 candidate_site in new List<Vector3>(candidate_sites))
                if (!IsCandidateSiteValid(candidate_site))
                    candidate_sites.Remove(candidate_site);

            if (candidate_sites.Count > 0)
                break;
        }
        if (candidate_sites.Count == 0)
        {
            builder.Unit.Task = null;
            return;
        }


        //Define metrics with which to judge valid candidate sites.
        //Metrics measure "badness", i.e. worse => higher number

        System.Func<Vector3, float> ConnectivityMetric = delegate (Vector3 candidate_site)
        {
            float connectivity = EstimateResultantConnectivity(candidate_site);
            int new_connections = GetBlocksAbutted(candidate_site).Values.First().Values.Sum();

            if (connectivity > MaxBlockConnectivity)
                return new_connections;

            return -new_connections;
        };

        System.Func<Vector3, float> TargetDistanceMetric = delegate (Vector3 candidate_site)
        {
            return candidate_site.Distance(complex_center);
        };

        System.Func<Vector3, float> BuilderDistanceMetric = delegate (Vector3 candidate_site)
        {
            return candidate_site.Distance(builder.Unit.Physical.Position);
        };

        System.Func<Vector3, float> LengthMetric = delegate (Vector3 candidate_site)
        {
            Graph block_abutted = GetBlocksAbutted(candidate_site).Keys.First();

            return -block_abutted.Nodes
                .Max(node => block_abutted.Nodes
                .Max(other_node => node.GetPosition().Distance(other_node.GetPosition())));
        };

        System.Func<Vector3, float> SmoothnessMetric = delegate (Vector3 candidate_site)
        {
            Graph block_abutted = GetBlocksAbutted(candidate_site).Keys.First();

            return -block_abutted.Nodes
                .Sum(node => node.GetPosition().Distance(candidate_site)) /
                block_abutted.Nodes.Count();
        };

        List<System.Func<Vector3, float>> metrics = Utility.List(
            ConnectivityMetric,
            TargetDistanceMetric,
            BuilderDistanceMetric,
            LengthMetric,
            SmoothnessMetric);

        Dictionary<System.Func<Vector3, float>, float> metric_weights =
            new Dictionary<System.Func<Vector3, float>, float>();
        metric_weights[ConnectivityMetric] = 0.0f;
        metric_weights[TargetDistanceMetric] = 4.0f;
        metric_weights[BuilderDistanceMetric] = 4.0f;
        metric_weights[LengthMetric] = 3.0f;
        metric_weights[SmoothnessMetric] = 3.0f;


        //Score candidate construction sites by computing weighted 
        //metrics and summing. Then, find minimum scored candidate 
        //and update ConstructionSite of BuildTasks in complex

        Dictionary<Vector3, float> scores = new Dictionary<Vector3, float>();

        foreach (Vector3 candidate_site in candidate_sites)
        {
            scores[candidate_site] = 0;

            foreach (System.Func<Vector3, float> metric in metrics)
                if (metric_weights[metric] > 0)
                    scores[candidate_site] += metric(candidate_site) * metric_weights[metric];
        }

        List<Vector3> sorted_candidate_sites =
            candidate_sites.Sorted(candidate_site => scores[candidate_site]);

        Vector3 complex_displacement = sorted_candidate_sites.First() - complex_center;

        foreach (BuildTask build_task in complex)
            build_task.Input.PrimaryVariableName =
                Scene.Main.World.MemorizePosition(build_task.ConstructionSite +
                                                    complex_displacement);
    }
}
