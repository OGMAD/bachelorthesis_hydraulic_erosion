using UnityEngine;

public static class MeshGeneration
{
    public static void CreateLandscape(Vertex[,] vertices)
    {
        GameObject quad = new GameObject("Plane");
        MeshFilter meshfilter = quad.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer meshrenderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh mesh = new Mesh();

        AddVerticesToMesh(vertices, mesh);
        CalculateAndAddUVToMesh(vertices, mesh);
        CalculateTriangles(vertices, mesh);

        meshfilter.mesh = mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    private static void AddVerticesToMesh(Vertex[,] vertices, Mesh mesh)
    {
        Vector3[] verts = new Vector3[vertices.GetUpperBound(0) * vertices.GetUpperBound(1)];
        int counter = 0;
        for(int columnIndex = 0; columnIndex < vertices.GetUpperBound(0); columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < vertices.GetUpperBound(1); rowIndex++)
            {
                verts[counter] = new Vector3
                    (
                        vertices[columnIndex, rowIndex].XCoord, 
                        vertices[columnIndex, rowIndex].YCoord, 
                        vertices[columnIndex, rowIndex].ZCoord
                    );
                counter++;
            }
        }
        
        mesh.vertices = verts;
    }

    private static void CalculateAndAddUVToMesh(Vertex[,] vertices, Mesh mesh)
    {
        Vector2[] vertsuv = new Vector2[vertices.GetUpperBound(0) * vertices.GetUpperBound(1)];
        int counter = 0;
        for (int columnIndex = 0; columnIndex < vertices.GetUpperBound(0); columnIndex++)
        {
            for (int rowIndex = 0; rowIndex < vertices.GetUpperBound(1); rowIndex++)
            {
                vertsuv[counter] = new Vector2
                    (
                        vertices[columnIndex, rowIndex].XCoord, 
                        vertices[columnIndex, rowIndex].ZCoord
                    );
                counter++;
            }
        }

        mesh.uv = vertsuv;
    }
    private static void CalculateTriangles(Vertex[,] vertices, Mesh mesh) 
    {
        int[] tris = new int[((vertices.GetUpperBound(0) * vertices.GetUpperBound(1)) - vertices.GetUpperBound(1)) * 3 ];
        int[] singletris = new int[3];
        int counter = 0;
        for(int i = 0; i < mesh.vertices.Length - vertices.GetUpperBound(1); i++)
        {
            if (i % vertices.GetUpperBound(1) != 0)
            {
                if (i % 2 == 0)
                {
                    singletris = RightSiteTriangle(i, vertices.GetUpperBound(1));
                }
                else
                {
                    singletris = LeftSiteTriangle(i, vertices.GetUpperBound(1));
                }

                for (int j = 0; j < singletris.Length; j++)
                {
                    tris[counter] = singletris[j];
                    counter++;
                }
            }
        }
        
        mesh.triangles = tris;
    }

    private static int[] RightSiteTriangle(int i, int offset)
    {
        return new[] { i, i + 1, i + offset };
    }

    private static int[] LeftSiteTriangle(int i, int offset)
    {
        return new[] { i, i + offset, i + offset - 1 };
    }
}
