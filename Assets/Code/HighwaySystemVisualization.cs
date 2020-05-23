using UnityEngine;
using System.Collections;

public class HighwaySystemVisualization : MonoBehaviour
{
    public GraphVisualization OutgoingRoads,
                              IncomingRoads;

    void Start()
    {
        OutgoingRoads.GetEdgeWidth = IncomingRoads.GetEdgeWidth = 
            edge => Scene.Main.World.Asteroid.HighwaySystem.RoadWidth / 10;

        OutgoingRoads.GetEdgeColor = edge => Color.green.AlphaChangedTo(0.08f);
        IncomingRoads.GetEdgeColor = edge => Color.red.AlphaChangedTo(0.08f);

        OutgoingRoads.ApplyOffset = IncomingRoads.ApplyOffset =
            position => position + new Vector3(0, 4, 0);
    }

    void Update()
    {
        if (Scene.Main.World.Asteroid.HighwaySystem == null)
            return;

        if(this.IsModulusUpdate(3600))
        {
            OutgoingRoads.Graph = 
                Scene.Main.World.Asteroid.HighwaySystem.OutgoingRoads;

            IncomingRoads.Graph = 
                Scene.Main.World.Asteroid.HighwaySystem.IncomingRoads;
        }
    }
}
