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
    private List<Vertex>[] Paths;
    public float ErosionStrength = 0.25f;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            #region set Heightmap to Material
            DisplacementMaterial.mainTexture = StateNameController.HeightMapSpriteUniformated.texture;
            DisplacementMaterial.SetTexture("_ParallaxMap", StateNameController.HeightMapSpriteUniformated.texture);
            #endregion

            float PlaneSizeX = (float)StateNameController.HeightMapSpriteUniformated.texture.width / 1000.0f;
            float PlaneSizeZ = (float)StateNameController.HeightMapSpriteUniformated.texture.height / 1000.0f;

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
        #region calculate Paths
        Paths = new List<Vertex>[Iterations];

        Parallel.For(0, Paths.Length, i =>
        {
            Paths[i] = new List<Vertex>();
        });

        System.Random rnd = new System.Random();

        Parallel.For(0, Iterations, i => 
        {
            #region get random Vertex as Start Point
            //spot to enter wind direction
            System.Random rndX = new System.Random();
            int StartVertexX = rndX.Next(0, Vertices.GetLength(0) - 1);

            System.Random rndY = new System.Random();
            int StartVertexY = rndY.Next(0, Vertices.GetLength(1) - 1);
            #endregion

            Vertex CurrentVertex = Vertices[StartVertexX, StartVertexY];

            CalculatePath(i, (CurrentVertex, rnd.Next(0, 7)), 0);
        });
        #endregion

        #region Debug
        /*for(int i = 0; i< Paths.Length; i++)
        {
            for(int j = 0; j < Paths[i].Count; j++)
            {
                Debug.Log("Path: " + i + " Position: " + j + " Coordinates: (" + Paths[i][j].XCoord + "/" + Paths[i][j].ZCoord + "/" + Paths[i][j].YCoord + ")");
            }
        }*/
        #endregion

        #region Erode
        foreach(List<Vertex>Path in Paths)
        {
            Vertex LastVertex = null;
            for (int i = 0; i < (int)(10.0f * ErosionStrength); i++)
            {
                foreach (Vertex CurrentVertex in Path)
                {
                    if (LastVertex != null)
                    {
                        float Delta = LastVertex.YCoord - CurrentVertex.YCoord;
                        LastVertex.YCoord -= (Delta / 2) * ErosionStrength;
                        CurrentVertex.YCoord += (Delta / 2) * ErosionStrength;
                    }

                    LastVertex = CurrentVertex;
                }
            }
        }
        #endregion
        #region Display
        Texture2D newHeightMap = GenerateTexture();
        DisplacementMaterial.SetTexture("_ParallaxMap", newHeightMap);
        DisplacementMaterial.mainTexture = newHeightMap;
        #endregion
    }

    private Texture2D GenerateTexture()
    {
        Texture2D HeightMap = new Texture2D(Vertices.GetLength(0), Vertices.GetLength(1));

        for(int x = 0; x < Vertices.GetLength(0); x++)
        {
            for (int y = 0; y < Vertices.GetLength(1); y++)
            {
                float c = CalculateHeight(Vertices[x,y].YCoord, false);
                #region Debug
                /*if (x == Vertices.GetLength(1) / 2)
                {
                    if (y < 100)
                    {
                        Debug.Log(c);
                    }
                }*/
                #endregion
                HeightMap.SetPixel(x, y, new Color(c, c, c));
            }
        }
        HeightMap.Apply();
        return HeightMap;
    }

    private void CalculatePath(int PathIndex, (Vertex, int) CurrentVertexAndDirection, int depth)
    {
        #region Neighbour Debug Area
        /*
                Vertex v = CurrentVertex;
                Debug.Log
                    (
                    "("+ PathIndex + "/" + depth + "Koord: "+ "(" + v.XCoord +"-"+ v.YCoord+")" + "): " 
                    + v.NeighbourLeft.YCoord + " - " 
                    + v.NeighbourUpperLeft.YCoord + " - " 
                    + v.NeighbourUpper.YCoord + " - " 
                    + v.NeighbourUpperRight.YCoord + " - " 
                    + v.NeighbourRight.YCoord + " - " 
                    + v.NeighbourLowerRight.YCoord + " - " 
                    + v.NeighbourLower.YCoord + " - " 
                    + v.NeighbourLowerLeft.YCoord
                    );
        */
        #endregion

        //Debug.Log("height: " + CurrentVertex.YCoord + "pixel color: " + CalculateHeight(CurrentVertex.YCoord, false));

        Paths[PathIndex].Add(CurrentVertexAndDirection.Item1);

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
            if (CurrentVertexAndDirection.Item1.YCoord >= NextVertex.Item1.YCoord && depth < 100)
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

        Vertices = CreateVertices(Vertices);
        FindNeighbours(Vertices);
    }

    private Vertex[,] CreateVertices(Vertex[,] vertices)
    {
        for (int column = 0; column < HeightMap.texture.width; column++)
        {
            for (int row = 0; row < HeightMap.texture.height; row++)
            {
                vertices[row, column] = new Vertex(column, row, CalculateHeight(HeightMap.texture.GetPixel(row, column).r,true));
            }
        }
        return vertices;
    }

    private void FindNeighbours(Vertex[,] Vertices)
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
