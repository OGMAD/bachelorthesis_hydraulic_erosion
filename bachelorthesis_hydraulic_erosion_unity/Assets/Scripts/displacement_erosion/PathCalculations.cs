using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathCalculations : MonoBehaviour
{
    public List<Vertex>[] Paths { get; set; }

    public void CalculatePath(int PathIndex, (Vertex, int) CurrentVertexAndDirection, int depth)
    {
        #region Draw Traces
        Paths[PathIndex].Add(CurrentVertexAndDirection.Item1);
        if (depth < 10)
        {
            CurrentVertexAndDirection.Item1.PathTrace = 10.0f;
        }
        else
        {
            CurrentVertexAndDirection.Item1.PathTrace = 1.0f;
        }
        #endregion

        if (CurrentVertexAndDirection.Item1.ReturnFalseIfSomeNeighbourIsNull())
        {
            (Vertex, int) NextVertexAndDirection = CurrentVertexAndDirection.Item1.ClaculateNextVertex(CurrentVertexAndDirection.Item2);
            if (CurrentVertexAndDirection.Item1.YCoord >= NextVertexAndDirection.Item1.YCoord)
            {
                CalculatePath(PathIndex, NextVertexAndDirection, (depth + 1));
            }
        }
    }

}
