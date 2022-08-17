using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vertex
{
    public Quad QuadUpperLeft;
    public Quad QuadUpperRight;
    public Quad QuadLowerRight;
    public Quad QuadLowerLeft;

    public Vertex NeighbourUpperLeft;
    public Vertex NeighbourUpper;
    public Vertex NeighbourUpperRight;
    public Vertex NeighbourRight;
    public Vertex NeighbourLowerRight;
    public Vertex NeighbourLower;
    public Vertex NeighbourLowerLeft;
    public Vertex NeighbourLeft;

    public int XCoord { get; set; }
    private int _yCoord;
    public int YCoord
    {
        get { return _yCoord; }
        set
        {
            _yCoord = value;
            RenderQuads();
        }
    }
    public int ZCoord { get; set; }

    public Vertex(int xWidth, int zDepth, int yHeight)
    {
        XCoord = xWidth;
        ZCoord = zDepth;
        YCoord = yHeight;
    }

    private void RenderQuads()
    {
        QuadUpperLeft.RenderQuad();
        QuadUpperRight.RenderQuad();
        QuadLowerRight.RenderQuad();
        QuadLowerLeft.RenderQuad();
    }
}
