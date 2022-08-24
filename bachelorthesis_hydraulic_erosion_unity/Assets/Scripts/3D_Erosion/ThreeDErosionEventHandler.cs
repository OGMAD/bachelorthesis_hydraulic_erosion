using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDErosionEventHandler : MonoBehaviour
{
    private Sprite HeightMap;
    private static int HighestPoint;
    private static int LowestPoint;

    // Start is called before the first frame update
    void Start()
    {
        GetAndSetVariables();
        TransferHightmapToObjects();
        //MeshGenerationHelper.CreatePlane();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetAndSetVariables()
    {
        HeightMap = StateNameController.HeightMapSpriteUniformated;
        HighestPoint = StateNameController.HighestPoint;
        LowestPoint = StateNameController.LowestPoint;
    }

    private void TransferHightmapToObjects()
    {
        Vertex[,] vertices = new Vertex[HeightMap.texture.height, HeightMap.texture.width];

        vertices = CreateVertices(vertices);
        FindNeighbours(vertices);
        CreateQuadsAndLinkThemToTheVertices(vertices);
    }

    private Vertex[,] CreateVertices(Vertex[,] vertices)
    {
        for (int column = 0; column < HeightMap.texture.width; column++)
        {
            for (int row = 0; row < HeightMap.texture.height; row++)
            {
                vertices[row, column] = new Vertex(column, row, CalculateHeight(HeightMap.texture.GetPixel(row, column).r));
            }
        }
        return vertices;
    }

    private void FindNeighbours(Vertex[,] Vertices)
    {
        for (int Column = 0; Column < HeightMap.texture.width; Column++)
        {
            for (int Row = 0; Row < HeightMap.texture.height; Row++)
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
                    else if (Column == HeightMap.texture.width - 1)
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
                else if (Row == HeightMap.texture.height - 1)
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
                    else if (Column == HeightMap.texture.width - 1)
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
                    else if (Column == HeightMap.texture.width - 1)
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
            }
        }
    }

    private void CreateQuadsAndLinkThemToTheVertices(Vertex[,] Vertices)
    {
        List<Quad> quads = new List<Quad>();
        for (int Column = 0; Column < HeightMap.texture.width - 1; Column++)
        {
            for (int Row = 0; Row < HeightMap.texture.height - 1; Row++)
            {
                quads.Add(new Quad(Vertices[Row, Column], Vertices[Row + 1, Column], Vertices[Row + 1, Column + 1], Vertices[Row, Column + 1]));
                Vertices[Row, Column].QuadLowerRight = quads[quads.Count - 1];
                Vertices[Row + 1, Column].QuadLowerLeft = quads[quads.Count - 1];
                Vertices[Row + 1, Column + 1].QuadUpperLeft = quads[quads.Count - 1];
                Vertices[Row, Column + 1].QuadUpperRight = quads[quads.Count - 1];
            }
        }
    }

    private int CalculateHeight(float Height)
    {
        return LowestPoint + (int)(Height * (float)HighestPoint);
    }
}
