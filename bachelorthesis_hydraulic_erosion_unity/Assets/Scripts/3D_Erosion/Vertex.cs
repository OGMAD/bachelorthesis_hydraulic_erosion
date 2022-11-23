using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public Vertex NeighbourLeft;
    public Vertex NeighbourUpperLeft;
    public Vertex NeighbourUpper;
    public Vertex NeighbourUpperRight;
    public Vertex NeighbourRight;
    public Vertex NeighbourLowerRight;
    public Vertex NeighbourLower;
    public Vertex NeighbourLowerLeft;
    
    public int XCoord { get; set; }
    private float _yCoord;
    public float YCoord
    {
        get { return _yCoord; }
        set
        {
            _yCoord = value;
        }
    }
    public int ZCoord { get; set; }

    public Vertex(int xWidth, int zDepth, float yHeight)
    {
        XCoord = xWidth;
        ZCoord = zDepth;
        YCoord = yHeight;
    }

    private List<Vertex> GetNeighbours()
    {
        List<Vertex> Neighbours = new List<Vertex>();

        if (NeighbourLeft != null)
        {
            Neighbours.Add(NeighbourLeft);
        }
        if (NeighbourUpperLeft != null)
        {
            Neighbours.Add(NeighbourUpperLeft);
        }
        if (NeighbourUpper != null)
        {
            Neighbours.Add(NeighbourUpper);
        }
        if (NeighbourUpperRight != null)
        {
            Neighbours.Add(NeighbourUpperRight);
        }

        if (NeighbourRight != null)
        {
            Neighbours.Add(NeighbourRight);
        }
        if (NeighbourLowerRight != null)
        {
            Neighbours.Add(NeighbourLowerRight);
        }
        if (NeighbourLower != null)
        {
            Neighbours.Add(NeighbourLower);
        }
        if (NeighbourLowerLeft != null)
        {
            Neighbours.Add(NeighbourLowerLeft);
        }
        return Neighbours;
    }

    public (Vertex, int) ClaculateNextVertex(int Direction)
    {
        List<Vertex> Neighbours = GetNeighbours();
        Neighbours.Sort(delegate (Vertex x, Vertex y) { return x.YCoord.CompareTo(y.YCoord); });

        float LowestNeighbour = Neighbours[0].YCoord;

        #region check if there is an lowest neighbour in direction
        if (Direction == 0 && NeighbourLeft.YCoord == LowestNeighbour)
        {
            return (NeighbourRight, Direction);
        }
        else if (Direction == 1 && NeighbourUpperLeft.YCoord == LowestNeighbour)
        {
            return (NeighbourLowerRight, Direction);
        }
        else if (Direction == 2 && NeighbourUpper.YCoord == LowestNeighbour)
        {
            return (NeighbourLower, Direction);
        }
        else if (Direction == 3 && NeighbourUpperRight.YCoord == LowestNeighbour)
        {
            return (NeighbourLowerLeft, Direction);
        }
        else if (Direction == 4 && NeighbourRight.YCoord == LowestNeighbour)
        {
            return (NeighbourLeft, Direction);
        }
        else if (Direction == 5 && NeighbourLowerRight.YCoord == LowestNeighbour)
        {
            return (NeighbourUpperLeft, Direction);
        }
        else if (Direction == 6 && NeighbourLower.YCoord == LowestNeighbour)
        {
            return (NeighbourUpper, Direction);
        }
        else if (Direction == 7 && NeighbourLowerLeft.YCoord == LowestNeighbour)
        {
            return (NeighbourUpperRight, Direction);
        }
        #endregion
        #region if not give to lowest with new direction
        if (NeighbourLeft.YCoord == LowestNeighbour)
        {
            return (NeighbourLeft, 4);
        }
        else if (NeighbourUpperLeft.YCoord == LowestNeighbour)
        {
            return (NeighbourUpperLeft, 5);
        }
        else if (NeighbourUpper.YCoord == LowestNeighbour)
        {
            return (NeighbourUpper, 6);
        }
        else if (NeighbourUpperRight.YCoord == LowestNeighbour)
        {
            return (NeighbourUpperRight, 7);
        }
        else if (NeighbourRight.YCoord == LowestNeighbour)
        {
            return (NeighbourRight, 0);
        }
        else if (NeighbourLowerRight.YCoord == LowestNeighbour)
        {
            return (NeighbourLowerRight, 1);
        }
        else if (NeighbourLower.YCoord == LowestNeighbour)
        {
            return (NeighbourLower, 2);
        }
        else
        {
            return (NeighbourLowerLeft, 3);
        }
        #endregion

    }
}
