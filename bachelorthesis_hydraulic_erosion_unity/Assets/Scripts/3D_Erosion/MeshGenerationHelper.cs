using UnityEngine;

public static class MeshGenerationHelper
{
    public static void CreatePlane()
    {
        GameObject quad = new GameObject("Plane");
        MeshFilter meshfilter = quad.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MeshRenderer meshrenderer = quad.AddComponent(typeof(MeshRenderer)) as MeshRenderer;

        Mesh mesh = new Mesh();

        float tl = 0.0f;
        float bl = 0.0f;
        float tr = 0.0f;
        float br = 0.0f;

        mesh.vertices = new Vector3[]
        {
            //width, height, depth
            new Vector3(0,tl,1),
            new Vector3(1,tr,1),
            new Vector3(1,br,0),
            new Vector3(0,bl,0)
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
