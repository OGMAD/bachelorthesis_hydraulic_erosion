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
    private static int HighestPoint;
    private static int LowestPoint;
    private Vertex[,] Vertices;
    private bool[,] PathTraces;
    private List<Vertex>[] Paths;

    private int Threadcount = 16;
    public bool ShouldEmbedErosionInLandscape = false;
    public float MaximalDropCapacity = 0.001f;

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

        System.Random rnd = new System.Random();
        Paths = new List<Vertex>[IterationsPerRun];
        #endregion

        Parallel.For(0, Paths.Length, i =>
        {
            Paths[i] = new List<Vertex>();
        });

        for (int RI = 0; RI < Runiterations; RI++)
        {
            #region calculate Paths
            Parallel.For(0, IterationsPerRun, i =>
            {
                //Debug.Log(rnd.Next(0, Vertices.GetLength(0) - 1) + " - " + rnd.Next(0, Vertices.GetLength(1) - 1));
                Vertex CurrentVertex = Vertices[rnd.Next(0, Vertices.GetLength(0) - 1), rnd.Next(0, Vertices.GetLength(1) - 1)];
                CalculatePath(i, (CurrentVertex, rnd.Next(0, 7)), 0);
            });
            #endregion
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
                            EmbedErosionInLandscape(LastVertex, CurrentVertex);
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
        #region Display
        Texture2D newHeightMap = GenerateTexture();
        Texture2D PathMap = GeneratePathTexture();

        DisplacementMaterial.SetTexture("_ParallaxMap", newHeightMap);
        DisplacementMaterial.mainTexture = PathMap;
        #endregion
    }

    private void EmbedErosionInLandscape(Vertex LastVertex, Vertex CurrentVertex)
    {
        (Vertex, Vertex) LeftAndRightNeighbour = GetLeftAndRightNeighbourBasedOnDirections(LastVertex, CurrentVertex);
        Vertex LeftNeighbour = LeftAndRightNeighbour.Item1;
        Vertex RightNeighbour = LeftAndRightNeighbour.Item2;

        if (LeftNeighbour != null && RightNeighbour != null)
        {
            #region Calculate Left
            float VLC = LastVertex.YCoord;
            float VL = LeftNeighbour.YCoord;

            LeftNeighbour.YCoord += (VLC - VL) * (1.0f / 3.0f);
            float CenterChangeLeft = (VLC - VL) * (2.0f / 3.0f);
            #endregion

            #region Calculate Right
            float VRC = LastVertex.YCoord;
            float VR = RightNeighbour.YCoord;

            LeftNeighbour.YCoord += (VRC - VR) * (1.0f / 3.0f);
            float CenterChangeRight = (VRC - VR) * (2.0f / 3.0f);
            #endregion

            #region Set Center
            LastVertex.YCoord += (CenterChangeLeft + CenterChangeRight);
            #endregion
        }
    }

    private (Vertex, Vertex) GetLeftAndRightNeighbourBasedOnDirections(Vertex LastVertex, Vertex CurrentVertex)
    {
        if (CurrentVertex == LastVertex.NeighbourLeft)
        {
            return (LastVertex.NeighbourUpper, LastVertex.NeighbourLower);
        }
        else if (CurrentVertex == LastVertex.NeighbourUpperLeft)
        {
            return (LastVertex.NeighbourUpperRight, LastVertex.NeighbourLowerLeft);
        }
        else if (CurrentVertex == LastVertex.NeighbourUpper)
        {
            return (LastVertex.NeighbourRight, LastVertex.NeighbourLeft);
        }
        else if (CurrentVertex == LastVertex.NeighbourUpperRight)
        {
            return (LastVertex.NeighbourLowerRight, LastVertex.NeighbourUpperLeft);
        }
        else if (CurrentVertex == LastVertex.NeighbourRight)
        {
            return (LastVertex.NeighbourLower, LastVertex.NeighbourUpper);
        }
        else if (CurrentVertex == LastVertex.NeighbourLowerRight)
        {
            return (LastVertex.NeighbourLowerLeft, LastVertex.NeighbourUpperRight);
        }
        else if (CurrentVertex == LastVertex.NeighbourLower)
        {
            return (LastVertex.NeighbourLeft, LastVertex.NeighbourRight);
        }
        else
        {
            return (LastVertex.NeighbourUpperLeft, LastVertex.NeighbourLowerRight);
        }
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

    private Texture2D GeneratePathTexture()
    {
        Texture2D HeightMap = new Texture2D(Vertices.GetLength(0), Vertices.GetLength(1));

        for (int x = 0; x < Vertices.GetLength(0); x++)
        {
            for (int y = 0; y < Vertices.GetLength(1); y++)
            {
                float r;
                if (PathTraces[x,y] == true)
                {
                    r = 1.0f;
                }
                else
                {
                    r = 0.0f;
                }
                HeightMap.SetPixel(x, y, new Color(r, 0, 0));
            }
        }
        HeightMap.Apply();
        return HeightMap;
    }

    private void CalculatePath(int PathIndex, (Vertex, int) CurrentVertexAndDirection, int depth)
    {
        Paths[PathIndex].Add(CurrentVertexAndDirection.Item1);
        PathTraces[CurrentVertexAndDirection.Item1.XCoord, CurrentVertexAndDirection.Item1.ZCoord] = true;

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
            if (CurrentVertexAndDirection.Item1.YCoord >= NextVertex.Item1.YCoord && depth < 50)
            {
                CalculatePath(PathIndex, NextVertex, (depth + 1));
            }
        }
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
        PathTraces = new bool[HeightMap.texture.height, HeightMap.texture.width];

        Vertices = CreateVertices(Vertices);
        FindNeighbours(Vertices, HeightMap);
    }

    private Vertex[,] CreateVertices(Vertex[,] vertices)
    {
        for (int column = 0; column < HeightMap.texture.width; column++)
        {
            for (int row = 0; row < HeightMap.texture.height; row++)
            {
                vertices[row, column] = new Vertex(column, row, CalculateHeight(HeightMap.texture.GetPixel(row, column).r, true));
                PathTraces[row, column] = false;
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
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Lower.Item2];
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
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Lower.Item2];
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
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Lower.Item2];
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
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Lower.Item2];
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
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Lower.Item2];
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
                        Vertices[Row, Column].NeighbourLeft = Vertices[Left.Item1, Lower.Item2];
                    }
                }
            });
        });

        #region Neighbour Debug Area
        /*
         for (int x = 0; x < Vertices.GetLength(0); x++)
        {
            for (int y = 0; y < Vertices.GetLength(1); y++)
            {
                Vertex v = Vertices[x, y];
                Debug.Log
                    (
                    "("+x+"/"+y+"): " 
                    + v.NeighbourLeft + " - " 
                    + v.NeighbourUpperLeft + " - " 
                    + v.NeighbourUpper + " - " 
                    + v.NeighbourUpperRight + " - " 
                    + v.NeighbourRight + " - " 
                    + v.NeighbourLowerRight + " - " 
                    + v.NeighbourLower + " - " 
                    + v.NeighbourLowerLeft
                    );
            }
        }
         */
        #endregion
    }

    private float CalculateHeight(float Height, bool ToObject)
    {
        int delta = HighestPoint - LowestPoint;
        if (ToObject)
        {

            return (float)Height * (float)delta;
        }
        else
        {
            return (float)Height / (float)delta;
        }

    }
    #endregion
}
