using UnityEngine;
using System.Collections;

public class HighwaySystemVisualization : MonoBehaviour
{
    public GraphVisualization HighwayGraphVisualization;

    void Start()
    {
        HighwayGraphVisualization.GetEdgeWidth =
            edge => Scene.Main.World.Asteroid.HighwaySystem.RoadWidth / 10;

        HighwayGraphVisualization.GetEdgeColor = edge => Color.yellow.AlphaChangedTo(0.4f);

        HighwayGraphVisualization.ApplyOffset = 
            position => position + new Vector3(0, 4, 0);
    }

    void Update()
    {
        if (Scene.Main.World.Asteroid.HighwaySystem == null)
            return;

        if(this.IsModulusUpdate(3600))
            HighwayGraphVisualization.Graph =
                Scene.Main.World.Asteroid.HighwaySystem.Highways;
    }
}
