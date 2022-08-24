using UnityEngine;

public class Quad
{
    public Vertex TopLeftVertex;
    public Vertex TopRightVertex;
    public Vertex BottomLeftVertex;
    public Vertex BottomRightVertex;

    public Quad(Vertex TopLeft, Vertex TopReight, Vertex BottomRight, Vertex BottomLeft)
    {
        TopLeftVertex = TopLeft;
        TopRightVertex = TopReight;
        BottomLeftVertex = BottomLeft;
        BottomRightVertex = BottomRight;
        RenderQuad();
    }

    public void RenderQuad()
    {
        GameObject quad = new GameObject("Plane");
        MeshFilter meshfilter = quad.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer meshrenderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        Mesh mesh = new Mesh();

        mesh.vertices = new Vector3[]
        {
            //width, height, depth
            new Vector3(TopLeftVertex.XCoord,TopLeftVertex.YCoord,TopLeftVertex.ZCoord),
            new Vector3(TopRightVertex.XCoord,TopRightVertex.YCoord,TopRightVertex.ZCoord),
            new Vector3(BottomRightVertex.XCoord,BottomRightVertex.YCoord,BottomRightVertex.ZCoord),
            new Vector3(BottomLeftVertex.XCoord,BottomLeftVertex.YCoord,BottomLeftVertex.ZCoord)
        };

        mesh.uv = new Vector2[]
        {
            new Vector2(0,0),
            new Vector2(0,1),
            new Vector2(1,1),
            new Vector2(1,0)
        };
        mesh.triangles = new int[] { 0, 1, 2, 0, 2, 3 };

        meshfilter.mesh = mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }
}