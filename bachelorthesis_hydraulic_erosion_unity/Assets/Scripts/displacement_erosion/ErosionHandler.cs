using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ErosionHandler : MonoBehaviour
{
    public Material DisplacementMaterial;
    public GameObject Plane;
    public string IterationsString { get; set; }
    private int Iterations = 0;

    private Sprite HeightMap;
    public float HighestPoint;
    public float LowestPoint;
    private Vertex[,] Vertices;
    private List<Vertex>[] Paths;

    private int Threadcount = 16;
    public bool ShouldEmbedErosionInLandscape = true;
    public float MaximalDropCapacity = 0.001f;
    PathCalculations PathCalculations;
    Embet Embet;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            #region set Heightmap to Material
            DisplacementMaterial.mainTexture = StateNameController.HeightMapSpriteUniformated.texture;
            DisplacementMaterial.SetTexture("_ParallaxMap", StateNameController.HeightMapSpriteUniformated.texture);
            #endregion

            float PlaneSizeX = (float)StateNameController.HeightMapSpriteUniformated.texture.width / 10000.0f;
            float PlaneSizeZ = (float)StateNameController.HeightMapSpriteUniformated.texture.height / 10000.0f;

            Plane.transform.localScale = new Vector3(PlaneSizeX, 1.0f, PlaneSizeZ);

            Objectify();

            PathCalculations = new();
            Embet = new();
        }
        catch
        {
            Debug.Log("Scene Test Mode");
        }

    }

    public void Simulate()
    {
        #region get Iterations
        try
        {
            Iterations = int.Parse(IterationsString);
        }
        catch
        {
            Debug.Log("Iterations == null");
        }
        #endregion

        #region Variabledeclerations
        int IterationsPerRun = Threadcount;
        int Runiterations = Iterations / IterationsPerRun;

        Paths = new List<Vertex>[IterationsPerRun];
        #endregion

        Parallel.For(0, Paths.Length, i =>
        {
            Paths[i] = new List<Vertex>();
        });
        PathCalculations.Paths = Paths;

        for (int RI = 0; RI < Runiterations; RI++)
        {
            CalculatePaths(IterationsPerRun);

            Erode();
        }
        #region Display
        Texture2D newHeightMap = GenerateTexture();
        Texture2D PathMap = GeneratePathTexture(newHeightMap);

        DisplacementMaterial.SetTexture("_ParallaxMap", newHeightMap);
        DisplacementMaterial.mainTexture = PathMap;
        #endregion
    }

    private void CalculatePaths(int IterationsPerRun)
    {
        System.Random rnd = new System.Random();

        Parallel.For(0, IterationsPerRun, i =>
        {
            //Debug.Log(rnd.Next(0, Vertices.GetLength(0) - 1) + " - " + rnd.Next(0, Vertices.GetLength(1) - 1));
            Vertex CurrentVertex = Vertices[rnd.Next(0, Vertices.GetLength(0) - 1), rnd.Next(0, Vertices.GetLength(1) - 1)];
            PathCalculations.CalculatePath(i, (CurrentVertex, rnd.Next(0, 7)), 0);
        });
        Paths = PathCalculations.Paths;
    }

    private void Erode()
    {
        #region Erode
        Parallel.For(0, Paths.Length, Path =>
        {
            //Debug.Log("Outer " + Path);
            Vertex LastVertex = null;
            for (int Vertex = 0; Vertex < Paths[Path].Count; Vertex++)
            {
                //Debug.Log("Inner " + Path + " " + Vertex);
                Vertex CurrentVertex = Paths[Path][Vertex];
                if (LastVertex != null)
                {
                    if (ShouldEmbedErosionInLandscape)
                    {
                        Embet.EmbedErosionInLandscape(LastVertex, CurrentVertex);
                    }
                    /*
                    float Delta = LastVertex.YCoord - CurrentVertex.YCoord;
                    float TransferredMaterial = (Delta / 4.0f);

                    if (TransferredMaterial > MaximalDropCapacity)
                    {
                        TransferredMaterial = MaximalDropCapacity;
                    }
                    //Debug.Log(TransferredMaterial);
                    LastVertex.YCoord -= TransferredMaterial;
                    CurrentVertex.YCoord += TransferredMaterial;
                    */
                }
                LastVertex = CurrentVertex;
            }
        });
        #endregion
    }
    private Texture2D GenerateTexture()
    {
        Texture2D HeightMap = new Texture2D(Vertices.GetLength(0), Vertices.GetLength(1));

        for (int x = 0; x < Vertices.GetLength(0); x++)
        {
            for (int y = 0; y < Vertices.GetLength(1); y++)
            {
                float c = CalculateHeight(Vertices[x, y].YCoord, false);
                HeightMap.SetPixel(x, y, new Color(c, c, c));
            }
        }
        HeightMap.Apply();
        return HeightMap;
    }

    private Texture2D GeneratePathTexture(Texture2D HeightMap)
    {
        Texture2D HeightMapWithPaths = new Texture2D(Vertices.GetLength(0), Vertices.GetLength(1));

        for (int x = 0; x < Vertices.GetLength(0); x++)
        {
            for (int y = 0; y < Vertices.GetLength(1); y++)
            {
                float r;
                float g;
                if (Vertices[x, y].PathTrace != 0.0f)
                {
                    if((Vertices[x, y].PathTrace == 10.0f))
                    {
                        g = 1.0f;
                        HeightMapWithPaths.SetPixel(x, y, new Color(0, g, 0));
                    }
                    else
                    {
                        r = 1.0f * Vertices[x, y].PathTrace;
                        HeightMapWithPaths.SetPixel(x, y, new Color(r, 0, 0));
                    }
                }
                else
                {
                    HeightMapWithPaths.SetPixel(x, y, HeightMap.GetPixel(x, y));
                }
            }
        }
        HeightMapWithPaths.Apply();
        return HeightMapWithPaths;
    }
    public void Save()
    {
        Debug.Log("Save!!!");
    }


    #region ObjectifyHeightMap
    public void Objectify()
    {
        GetAndSetVariables();
        TransferHightmapToObjects();
    }
    private void GetAndSetVariables()
    {
        HeightMap = StateNameController.HeightMapSpriteUniformated;
        HighestPoint = StateNameController.HighestPoint;
        LowestPoint = StateNameController.LowestPoint;
    }

    private void TransferHightmapToObjects()
    {
        Vertices = new Vertex[HeightMap.texture.height, HeightMap.texture.width];

        Vertices = CreateVertices(Vertices);
        FindNeighbours(Vertices, HeightMap);
        DisplacementMaterial.mainTexture = GeneratePathTexture(HeightMap.texture);
    }

    private Vertex[,] CreateVertices(Vertex[,] vertices)
    {
        for (int column = 0; column < HeightMap.texture.width; column++)
        {
            for (int row = 0; row < HeightMap.texture.height; row++)
            {
                vertices[row, column] = new Vertex(column, row, CalculateHeight(HeightMap.texture.GetPixel(row, column).r, true));
            }
        }
        return vertices;
    }

    public void FindNeighbours(Vertex[,] Vertices, Sprite HeightMap)
    {
        int width = HeightMap.texture.width;
        int height = HeightMap.texture.height;
        Parallel.For(0, width, Column =>
        {
            Parallel.For(0, height, Row =>
            {
                (int, int) UpperLeft = (Row - 1, Column - 1);
                (int, int) Upper = (Row - 1, Column);
                (int, int) UpperRight = (Row - 1, Column + 1);
                (int, int) Right = (Row, Column + 1);
                (int, int) LowerRight = (Row + 1, Column + 1);
                (int, int) Lower = (Row + 1, Column);
                (int, int) LowerLeft = (Row + 1, Column - 1);
                (int, int) Left = (Row, Column - 1);

                if (Row == 0)
                {
                    if (Column == 0)
                    {
                        //first row & first column
                        //top left corner
                        Vertices[Row, Column].NeighbourUpperLeft = null;
                        Vertices[Row, Column].NeighbourUpper = null;
                        Vertices[Row, Column].NeighbourUpperRight = null;
                        Vertices[Row, Column].NeighbourRight = Vertices[Right.Item1, Right.Item2];
                        Vertices[Row, Column].NeighbourLowerRight = Vertices[LowerRight.Item1, LowerRight.Item2];
                        Vertices[Row, Column].NeighbourLower = Vertices[Lower.Item1, Lower.Item2];
                        Vertices[Row, Column].NeighbourLowerLeft = null;
                        Vertices[Row, Column].NeighbourLeft = null;
                    }
                    else if (Column == width - 1)
                    {
                        //first row & last column
                        //top right corner
                        Vertices[Row, Column].NeighbourUpperLeft = null;
                        Vertices[Row, Column].NeighbourUpper = null;
                        Vertices[Row, Column].NeighbourUpperRight = null;
                        Vertices[Row, Column].NeighbourRight = null;
                        Vertices[Row, Column].NeighbourLowerRight = null;
                        Vertices[Row, Column].NeighbourLower = Vertices[Lower.Item1, Lower.Item2];
                        Vertices[Row, Column].NeighbourLowerLeft = Vertices[LowerLeft.Item1, LowerLeft.Item2];
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Left.Item2];
                    }
                    else
                    {
                        //first row
                        //max top
                        Vertices[Row, Column].NeighbourUpperLeft = null;
                        Vertices[Row, Column].NeighbourUpper = null;
                        Vertices[Row, Column].NeighbourUpperRight = null;
                        Vertices[Row, Column].NeighbourRight = Vertices[Right.Item1, Right.Item2];
                        Vertices[Row, Column].NeighbourLowerRight = Vertices[LowerRight.Item1, LowerRight.Item2];
                        Vertices[Row, Column].NeighbourLower = Vertices[Lower.Item1, Lower.Item2];
                        Vertices[Row, Column].NeighbourLowerLeft = Vertices[LowerLeft.Item1, LowerLeft.Item2];
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Left.Item2];
                    }
                }
                else if (Row == height - 1)
                {
                    if (Column == 0)
                    {
                        //last row & first column
                        //bottom left corner
                        Vertices[Row, Column].NeighbourUpperLeft = null;
                        Vertices[Row, Column].NeighbourUpper = Vertices[Upper.Item1, Upper.Item2];
                        Vertices[Row, Column].NeighbourUpperRight = Vertices[UpperRight.Item1, UpperRight.Item2];
                        Vertices[Row, Column].NeighbourRight = Vertices[Right.Item1, Right.Item2];
                        Vertices[Row, Column].NeighbourLowerRight = null;
                        Vertices[Row, Column].NeighbourLower = null;
                        Vertices[Row, Column].NeighbourLowerLeft = null;
                        Vertices[Row, Column].NeighbourLeft = null;
                    }
                    else if (Column == width - 1)
                    {
                        //last row & last column
                        //bottom right corner
                        Vertices[Row, Column].NeighbourUpperLeft = Vertices[UpperLeft.Item1, UpperLeft.Item2];
                        Vertices[Row, Column].NeighbourUpper = Vertices[Upper.Item1, Upper.Item2];
                        Vertices[Row, Column].NeighbourUpperRight = null;
                        Vertices[Row, Column].NeighbourRight = null;
                        Vertices[Row, Column].NeighbourLowerRight = null;
                        Vertices[Row, Column].NeighbourLower = null;
                        Vertices[Row, Column].NeighbourLowerLeft = null;
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Left.Item2];
                    }
                    else
                    {
                        //last row
                        //max bottom
                        Vertices[Row, Column].NeighbourUpperLeft = Vertices[UpperLeft.Item1, UpperLeft.Item2];
                        Vertices[Row, Column].NeighbourUpper = Vertices[Upper.Item1, Upper.Item2];
                        Vertices[Row, Column].NeighbourUpperRight = Vertices[UpperRight.Item1, UpperRight.Item2];
                        Vertices[Row, Column].NeighbourRight = Vertices[Right.Item1, Right.Item2];
                        Vertices[Row, Column].NeighbourLowerRight = null;
                        Vertices[Row, Column].NeighbourLower = null;
                        Vertices[Row, Column].NeighbourLowerLeft = null;
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Left.Item2];
                    }
                }
                else
                {
                    if (Column == 0)
                    {
                        //first column
                        //max left
                        Vertices[Row, Column].NeighbourUpperLeft = null;
                        Vertices[Row, Column].NeighbourUpper = Vertices[Upper.Item1, Upper.Item2];
                        Vertices[Row, Column].NeighbourUpperRight = Vertices[UpperRight.Item1, UpperRight.Item2];
                        Vertices[Row, Column].NeighbourRight = Vertices[Right.Item1, Right.Item2];
                        Vertices[Row, Column].NeighbourLowerRight = Vertices[LowerRight.Item1, LowerRight.Item2];
                        Vertices[Row, Column].NeighbourLower = Vertices[Lower.Item1, Lower.Item2];
                        Vertices[Row, Column].NeighbourLowerLeft = null;
                        Vertices[Row, Column].NeighbourLeft = null;
                    }
                    else if (Column == width - 1)
                    {
                        //last column
                        //max right
                        Vertices[Row, Column].NeighbourUpperLeft = Vertices[UpperLeft.Item1, UpperLeft.Item2];
                        Vertices[Row, Column].NeighbourUpper = Vertices[Upper.Item1, Upper.Item2];
                        Vertices[Row, Column].NeighbourUpperRight = null;
                        Vertices[Row, Column].NeighbourRight = null;
                        Vertices[Row, Column].NeighbourLowerRight = null;
                        Vertices[Row, Column].NeighbourLower = Vertices[Lower.Item1, Lower.Item2];
                        Vertices[Row, Column].NeighbourLowerLeft = Vertices[LowerLeft.Item1, LowerLeft.Item2];
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Left.Item2];
                    }
                    else
                    {
                        //normal Vertex
                        Vertices[Row, Column].NeighbourUpperLeft = Vertices[UpperLeft.Item1, UpperLeft.Item2];
                        Vertices[Row, Column].NeighbourUpper = Vertices[Upper.Item1, Upper.Item2];
                        Vertices[Row, Column].NeighbourUpperRight = Vertices[UpperRight.Item1, UpperRight.Item2];
                        Vertices[Row, Column].NeighbourRight = Vertices[Right.Item1, Right.Item2];
                        Vertices[Row, Column].NeighbourLowerRight = Vertices[LowerRight.Item1, LowerRight.Item2];
                        Vertices[Row, Column].NeighbourLower = Vertices[Lower.Item1, Lower.Item2];
                        Vertices[Row, Column].NeighbourLowerLeft = Vertices[LowerLeft.Item1, LowerLeft.Item2];
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Left.Item2];
                    }
                }
            });
        });
    }

    public float CalculateHeight(float Height, bool ToObject)
    {
        float delta = HighestPoint - LowestPoint;
        if (ToObject)
        {

            return ((float)Height * (float)delta);
        }
        else
        {
            return ((float)Height / (float)delta);
        }

    }
    #endregion
}
