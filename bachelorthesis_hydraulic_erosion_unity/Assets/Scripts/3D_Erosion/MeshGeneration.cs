using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

public static class MeshGeneration
{
    private static Material Grass;
    public static void CreateLandscape(Vertex[,] vertices)
    {
        GetMaterials();
        
        GameObject Landscape = new GameObject("Plane");
        MeshFilter meshFilter = Landscape.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer meshRenderer = Landscape.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh mesh = new Mesh();

        AddVerticesToMesh(vertices, mesh);
        //CalculateAndAddUVToMesh(vertices, mesh);
        CalculateTriangles(vertices, mesh);

        meshFilter.mesh = mesh;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        Landscape.gameObject.GetComponent<Renderer>().material = Grass;
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
        
        int height = vertices.GetUpperBound(0);
        int width = vertices.GetUpperBound(1);
        
        //TODO:correct arry length
        int[] tris = new int[((((height) * (width)) - (width)) * 6)];
        
        int[] singletris = new int[3];

        Parallel.For(0, mesh.vertices.Length - width, i =>
        {
            //if(left edge)
            //if(right edge)
            //else(mid V)

            #region calculate x/y Pos of i

            int c = 0;
            int b = i;
            while (b > width)
            {
                b -= width;
                c++;
            }

            int XPos = b;
            int YPos = c;

            //Debug.Log(i + "=(" + XPos +"/ " + YPos + ")");
            #endregion

            #region calculate counter startpoint
            // +3 for every edge V
            // +6 for every other V
            int counter = 0;
            int ThreeVertices = 0;
            int SixVertices = 0;
            if (XPos == 0)
            {
                if (YPos == 0)
                {
                    ThreeVertices = 0;
                }
                else
                {
                    ThreeVertices = YPos * 2;
                }
            }
            else
            {
                if (YPos == 0)
                {
                    ThreeVertices = 1;
                    
                }
                else
                {
                    ThreeVertices = (YPos * 2) + 1;
                }
            }
            SixVertices = i - ThreeVertices;
            counter = (ThreeVertices * 3) + (SixVertices * 6);
            #endregion
            
            if ((i + width - 1) % width == 0)
            {
                singletris = RightSiteTriangle(i, width);
                for (int j = 0; j < singletris.Length; j++)
                {
                    tris[counter] = singletris[j];
                    counter++;
                }
            }
            else if(i % width == 0)
            {
                singletris = LeftSiteTriangle(i, width);
                for (int j = 0; j < singletris.Length; j++)
                {
                    tris[counter] = singletris[j];
                    counter++;
                }
            }
            else
            {
                singletris = RightSiteTriangle(i, width);
                for (int j = 0; j < singletris.Length; j++)
                {
                    tris[counter] = singletris[j];
                    counter++;
                }
                
                singletris = LeftSiteTriangle(i, width);
                for (int j = 0; j < singletris.Length; j++)
                {
                    tris[counter] = singletris[j];
                    counter++;
                }
            }
        });
        
        mesh.triangles = tris;
    }

    private static int[] RightSiteTriangle(int i, int offset)
    {
        return new[] { i, i + offset, i + 1 };
    }

    private static int[] LeftSiteTriangle(int i, int offset)
    {
        return new[] { i, i + offset - 1, i + offset };
    }
    private static void GetMaterials()
    {
        Grass = Resources.Load<Material>("Materials/Grass");
    }
}
