using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCalculations : MonoBehaviour
{
    public List<Vertex>[] Paths { get; set; }
    public float[,] PathTraces { get; set; }

    public void CalculatePath(int PathIndex, (Vertex, int) CurrentVertexAndDirection, int depth)
    {
        Paths[PathIndex].Add(CurrentVertexAndDirection.Item1);
        if (depth < 10)
        {
            PathTraces[CurrentVertexAndDirection.Item1.XCoord, CurrentVertexAndDirection.Item1.ZCoord] = 10.0f;
        }
        else
        {
            PathTraces[CurrentVertexAndDirection.Item1.XCoord, CurrentVertexAndDirection.Item1.ZCoord] = 1.0f;
        }


        if
            (
            CurrentVertexAndDirection.Item1.NeighbourLeft != null &&
            CurrentVertexAndDirection.Item1.NeighbourUpperLeft != null &&
            CurrentVertexAndDirection.Item1.NeighbourUpper != null &&
            CurrentVertexAndDirection.Item1.NeighbourUpperRight != null &&
            CurrentVertexAndDirection.Item1.NeighbourRight != null &&
            CurrentVertexAndDirection.Item1.NeighbourLowerRight != null &&
            CurrentVertexAndDirection.Item1.NeighbourLower != null &&
            CurrentVertexAndDirection.Item1.NeighbourLowerLeft != null
            )
        {
            (Vertex, int) NextVertex = CurrentVertexAndDirection.Item1.ClaculateNextVertex(CurrentVertexAndDirection.Item2);
            if (CurrentVertexAndDirection.Item1.YCoord >= NextVertex.Item1.YCoord)
            {
                CalculatePath(PathIndex, NextVertex, (depth + 1));
            }
        }
    }

}
