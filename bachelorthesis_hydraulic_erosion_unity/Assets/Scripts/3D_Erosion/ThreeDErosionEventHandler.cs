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
        MeshGenerationHelper.CreatePlane(5, 10);
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
        CreateQuadsAndLinkThemToTheVertices();
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

    private void FindNeighbours(Vertex[,] vertices)
    {
        for (int Column = 0; Column < HeightMap.texture.width; Column++)
        {
            for (int Row = 0; Row < HeightMap.texture.height; Row++)
            {
                if (Row == 0)
                {
                    if (Column == 0)
                    {
                        //first row & first column
                        vertices[Row, Column].NeighbourUpperLeft = null;
                        vertices[Row, Column].NeighbourUpper = null;
                        vertices[Row, Column].NeighbourUpperRight = null;
                        vertices[Row, Column].NeighbourRight = vertices[Row, Column + 1];
                        vertices[Row, Column].NeighbourLowerRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourLower = vertices[Row + 1, Column];
                        vertices[Row, Column].NeighbourLowerLeft = null;
                        vertices[Row, Column].NeighbourLeft = null;
                    }
                    else if (Column == HeightMap.texture.width - 1)
                    {
                        //first row & last column
                        vertices[Row, Column].NeighbourUpperLeft = null;
                        vertices[Row, Column].NeighbourUpper = null;
                        vertices[Row, Column].NeighbourUpperRight = null;
                        vertices[Row, Column].NeighbourRight = null;
                        vertices[Row, Column].NeighbourLowerRight = null;
                        vertices[Row, Column].NeighbourLower = vertices[Row + 1, Column];
                        vertices[Row, Column].NeighbourLowerLeft = vertices[Row + 1, Column - 1];
                        vertices[Row, Column].NeighbourLeft = vertices[Row, Column - 1];
                    }
                    else
                    {
                        //first row
                        vertices[Row, Column].NeighbourUpperLeft = null;
                        vertices[Row, Column].NeighbourUpper = null;
                        vertices[Row, Column].NeighbourUpperRight = null;
                        vertices[Row, Column].NeighbourRight = vertices[Row, Column + 1];
                        vertices[Row, Column].NeighbourLowerRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourLower = vertices[Row + 1, Column];
                        vertices[Row, Column].NeighbourLowerLeft = vertices[Row + 1, Column - 1];
                        vertices[Row, Column].NeighbourLeft = vertices[Row, Column - 1];
                    }
                }
                else if (Row == HeightMap.texture.height - 1)
                {
                    if (Column == 0)
                    {
                        //last row & first column
                        vertices[Row, Column].NeighbourUpperLeft = null;
                        vertices[Row, Column].NeighbourUpper = vertices[Row - 1, Column];
                        vertices[Row, Column].NeighbourUpperRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourRight = vertices[Row, Column + 1];
                        vertices[Row, Column].NeighbourLowerRight = null;
                        vertices[Row, Column].NeighbourLower = null;
                        vertices[Row, Column].NeighbourLowerLeft = null;
                        vertices[Row, Column].NeighbourLeft = null;
                    }
                    else if (Column == HeightMap.texture.width - 1)
                    {
                        //last row & last column
                        vertices[Row, Column].NeighbourUpperLeft = vertices[Row - 1, Column - 1];
                        vertices[Row, Column].NeighbourUpper = vertices[Row - 1, Column];
                        vertices[Row, Column].NeighbourUpperRight = null;
                        vertices[Row, Column].NeighbourRight = null;
                        vertices[Row, Column].NeighbourLowerRight = null;
                        vertices[Row, Column].NeighbourLower = null;
                        vertices[Row, Column].NeighbourLowerLeft = null;
                        vertices[Row, Column].NeighbourLeft = vertices[Row, Column - 1];
                    }
                    else
                    {
                        //last row
                        vertices[Row, Column].NeighbourUpperLeft = vertices[Row - 1, Column - 1];
                        vertices[Row, Column].NeighbourUpper = vertices[Row - 1, Column];
                        vertices[Row, Column].NeighbourUpperRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourRight = vertices[Row, Column + 1];
                        vertices[Row, Column].NeighbourLowerRight = null;
                        vertices[Row, Column].NeighbourLower = null;
                        vertices[Row, Column].NeighbourLowerLeft = null;
                        vertices[Row, Column].NeighbourLeft = vertices[Row, Column - 1];
                    }
                }
                else
                {
                    if (Column == 0)
                    {
                        //first column
                        vertices[Row, Column].NeighbourUpperLeft = null;
                        vertices[Row, Column].NeighbourUpper = vertices[Row - 1, Column];
                        vertices[Row, Column].NeighbourUpperRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourRight = vertices[Row, Column + 1];
                        vertices[Row, Column].NeighbourLowerRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourLower = vertices[Row + 1, Column];
                        vertices[Row, Column].NeighbourLowerLeft = null;
                        vertices[Row, Column].NeighbourLeft = null;
                    }
                    else if (Column == HeightMap.texture.width - 1)
                    {
                        //last column
                        vertices[Row, Column].NeighbourUpperLeft = vertices[Row - 1, Column - 1];
                        vertices[Row, Column].NeighbourUpper = vertices[Row - 1, Column];
                        vertices[Row, Column].NeighbourUpperRight = null;
                        vertices[Row, Column].NeighbourRight = null;
                        vertices[Row, Column].NeighbourLowerRight = null;
                        vertices[Row, Column].NeighbourLower = vertices[Row + 1, Column];
                        vertices[Row, Column].NeighbourLowerLeft = vertices[Row + 1, Column - 1];
                        vertices[Row, Column].NeighbourLeft = vertices[Row, Column - 1];
                    }
                    else
                    {
                        //normal Vertex
                        vertices[Row, Column].NeighbourUpperLeft = vertices[Row - 1, Column - 1];
                        vertices[Row, Column].NeighbourUpper = vertices[Row - 1, Column];
                        vertices[Row, Column].NeighbourUpperRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourRight = vertices[Row, Column + 1];
                        vertices[Row, Column].NeighbourLowerRight = vertices[Row + 1, Column + 1];
                        vertices[Row, Column].NeighbourLower = vertices[Row + 1, Column];
                        vertices[Row, Column].NeighbourLowerLeft = vertices[Row + 1, Column - 1];
                        vertices[Row, Column].NeighbourLeft = vertices[Row, Column - 1];
                    }
                }
            }
        }
    }
    private void CreateQuadsAndLinkThemToTheVertices()
    {
        for (int column = 0; column < HeightMap.texture.width; column++)
        {
            for (int row = 0; row < HeightMap.texture.height; row++)
            {
                if (row == 0)
                {
                    if (column == 0)
                    {
                        //first row & first column
                    }
                    else if (column == HeightMap.texture.width - 1)
                    {
                        //first row & last column
                    }
                    else
                    {
                        //first row
                    }
                }
                else if (row == HeightMap.texture.height - 1)
                {
                    if (column == 0)
                    {
                        //last row & first column
                    }
                    else if (column == HeightMap.texture.width - 1)
                    {
                        //last row & last column
                    }
                    else
                    {
                        //last row
                    }
                }
                else
                {
                    if (column == 0)
                    {
                        //first column
                    }
                    else if (column == HeightMap.texture.width - 1)
                    {
                        //last column
                    }
                    else
                    {
                        //normal Vertex
                    }
                }
            }
        }
    }

    private int CalculateHeight(float Height)
    {
        return LowestPoint + (int)(Height * (float)HighestPoint);
    }
}
