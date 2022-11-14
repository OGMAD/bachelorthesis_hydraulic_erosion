using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NUnit.Framework;

[TestFixture]
public class TestErosionHandler : MonoBehaviour
{
    private Sprite HeightMap = Sprite.Create(new Texture2D(4, 4), new Rect(), new Vector2());
    private Vertex[,] MockupVertices =
    {
        {new Vertex(0,0,0), new Vertex(0,1,1), new Vertex(0,2,2), new Vertex(0,3,3)},
        {new Vertex(1,0,4), new Vertex(1,1,5), new Vertex(1,2,6), new Vertex(1,3,7)},
        {new Vertex(2,0,8), new Vertex(2,1,9), new Vertex(2,2,10), new Vertex(2,3,11)},
        {new Vertex(3,0,12), new Vertex(3,1,13), new Vertex(3,2,14), new Vertex(3,3,15)}
    };

    [Test]
    public void FindNeighbours_Succsess()
    {
        System.Console.WriteLine("Hello World!");
        ErosionHandler erosionHandler = new ErosionHandler();
        erosionHandler.FindNeighbours(MockupVertices, HeightMap);
        bool FoundCorrectNeighbours = false;
        //0
        if
            (
            MockupVertices[0, 0].NeighbourLeft == null &&
            MockupVertices[0, 0].NeighbourUpperLeft == null &&
            MockupVertices[0, 0].NeighbourUpper == null &&
            MockupVertices[0, 0].NeighbourUpperRight == null &&
            MockupVertices[0, 0].NeighbourRight.YCoord == 1 &&
            MockupVertices[0, 0].NeighbourLowerRight.YCoord == 5 &&
            MockupVertices[0, 0].NeighbourLower.YCoord == 4 &&
            MockupVertices[0, 0].NeighbourLowerLeft == null
            )
        {
            //1
            if
            (
            MockupVertices[0, 1].NeighbourLeft.YCoord == 0 &&
            MockupVertices[0, 1].NeighbourUpperLeft == null &&
            MockupVertices[0, 1].NeighbourUpper == null &&
            MockupVertices[0, 1].NeighbourUpperRight == null &&
            MockupVertices[0, 1].NeighbourRight.YCoord == 2 &&
            MockupVertices[0, 1].NeighbourLowerRight.YCoord == 6 &&
            MockupVertices[0, 1].NeighbourLower.YCoord == 5 &&
            MockupVertices[0, 1].NeighbourLowerLeft.YCoord == 4
            )
            {
                //2
                if
            (
            MockupVertices[0, 2].NeighbourLeft.YCoord == 1 &&
            MockupVertices[0, 2].NeighbourUpperLeft == null &&
            MockupVertices[0, 2].NeighbourUpper == null &&
            MockupVertices[0, 2].NeighbourUpperRight == null &&
            MockupVertices[0, 2].NeighbourRight.YCoord == 3 &&
            MockupVertices[0, 2].NeighbourLowerRight.YCoord == 7 &&
            MockupVertices[0, 2].NeighbourLower.YCoord == 6 &&
            MockupVertices[0, 2].NeighbourLowerLeft.YCoord == 5
            )
                {
                    //3
                    if
            (
            MockupVertices[0, 3].NeighbourLeft.YCoord == 2 &&
            MockupVertices[0, 3].NeighbourUpperLeft == null &&
            MockupVertices[0, 3].NeighbourUpper == null &&
            MockupVertices[0, 3].NeighbourUpperRight == null &&
            MockupVertices[0, 3].NeighbourRight == null &&
            MockupVertices[0, 3].NeighbourLowerRight == null &&
            MockupVertices[0, 3].NeighbourLower.YCoord == 7 &&
            MockupVertices[0, 3].NeighbourLowerLeft.YCoord == 6
            )
                    {
                        //4
                        if
            (
            MockupVertices[1, 0].NeighbourLeft == null &&
            MockupVertices[1, 0].NeighbourUpperLeft == null &&
            MockupVertices[1, 0].NeighbourUpper.YCoord == 0 &&
            MockupVertices[1, 0].NeighbourUpperRight.YCoord == 1 &&
            MockupVertices[1, 0].NeighbourRight.YCoord == 5 &&
            MockupVertices[1, 0].NeighbourLowerRight.YCoord == 9 &&
            MockupVertices[1, 0].NeighbourLower.YCoord == 8 &&
            MockupVertices[1, 0].NeighbourLowerLeft == null
            )
                        {
                            //5
                            if
            (
            MockupVertices[1, 1].NeighbourLeft.YCoord == 4 &&
            MockupVertices[1, 1].NeighbourUpperLeft.YCoord == 0 &&
            MockupVertices[1, 1].NeighbourUpper.YCoord == 1 &&
            MockupVertices[1, 1].NeighbourUpperRight.YCoord == 2 &&
            MockupVertices[1, 1].NeighbourRight.YCoord == 6 &&
            MockupVertices[1, 1].NeighbourLowerRight.YCoord == 10 &&
            MockupVertices[1, 1].NeighbourLower.YCoord == 9 &&
            MockupVertices[1, 1].NeighbourLowerLeft.YCoord == 8
            )
                            {
                                //6
                                if
                                            (
                                            MockupVertices[1, 2].NeighbourLeft.YCoord == 5 &&
                                            MockupVertices[1, 2].NeighbourUpperLeft.YCoord == 1 &&
                                            MockupVertices[1, 2].NeighbourUpper.YCoord == 2 &&
                                            MockupVertices[1, 2].NeighbourUpperRight.YCoord == 3 &&
                                            MockupVertices[1, 2].NeighbourRight.YCoord == 7 &&
                                            MockupVertices[1, 2].NeighbourLowerRight.YCoord == 11 &&
                                            MockupVertices[1, 2].NeighbourLower.YCoord == 10 &&
                                            MockupVertices[1, 2].NeighbourLowerLeft.YCoord == 9
                                            )
                                {
                                    //7
                                    if
                                                (
                                                MockupVertices[1, 3].NeighbourLeft.YCoord == 6 &&
                                                MockupVertices[1, 3].NeighbourUpperLeft.YCoord == 2 &&
                                                MockupVertices[1, 3].NeighbourUpper.YCoord == 3 &&
                                                MockupVertices[1, 3].NeighbourUpperRight == null &&
                                                MockupVertices[1, 3].NeighbourRight == null &&
                                                MockupVertices[1, 3].NeighbourLowerRight == null &&
                                                MockupVertices[1, 3].NeighbourLower.YCoord == 11 &&
                                                MockupVertices[1, 3].NeighbourLowerLeft.YCoord == 10
                                                )
                                    {
                                        //8
                                        if
                                                    (
                                                    MockupVertices[2, 0].NeighbourLeft == null &&
                                                    MockupVertices[2, 0].NeighbourUpperLeft == null &&
                                                    MockupVertices[2, 0].NeighbourUpper.YCoord == 4 &&
                                                    MockupVertices[2, 0].NeighbourUpperRight.YCoord == 5 &&
                                                    MockupVertices[2, 0].NeighbourRight.YCoord == 9 &&
                                                    MockupVertices[2, 0].NeighbourLowerRight.YCoord == 13 &&
                                                    MockupVertices[2, 0].NeighbourLower.YCoord == 12 &&
                                                    MockupVertices[2, 0].NeighbourLowerLeft == null
                                                    )
                                        {
                                            //9
                                            if
                                                        (
                                                        MockupVertices[2, 1].NeighbourLeft.YCoord == 8 &&
                                                        MockupVertices[2, 1].NeighbourUpperLeft.YCoord == 4 &&
                                                        MockupVertices[2, 1].NeighbourUpper.YCoord == 5 &&
                                                        MockupVertices[2, 1].NeighbourUpperRight.YCoord == 6 &&
                                                        MockupVertices[2, 1].NeighbourRight.YCoord == 10 &&
                                                        MockupVertices[2, 1].NeighbourLowerRight.YCoord == 14 &&
                                                        MockupVertices[2, 1].NeighbourLower.YCoord == 13 &&
                                                        MockupVertices[2, 1].NeighbourLowerLeft.YCoord == 12
                                                        )
                                            {
                                                //10
                                                if
                                                            (
                                                            MockupVertices[2, 2].NeighbourLeft.YCoord == 9 &&
                                                            MockupVertices[2, 2].NeighbourUpperLeft.YCoord == 5 &&
                                                            MockupVertices[2, 2].NeighbourUpper.YCoord == 6 &&
                                                            MockupVertices[2, 2].NeighbourUpperRight.YCoord == 7 &&
                                                            MockupVertices[2, 2].NeighbourRight.YCoord == 11 &&
                                                            MockupVertices[2, 2].NeighbourLowerRight.YCoord == 15 &&
                                                            MockupVertices[2, 2].NeighbourLower.YCoord == 14 &&
                                                            MockupVertices[2, 2].NeighbourLowerLeft.YCoord == 13
                                                            )
                                                {
                                                    //11
                                                    if
                                                                (
                                                                MockupVertices[2, 3].NeighbourLeft.YCoord == 10 &&
                                                                MockupVertices[2, 3].NeighbourUpperLeft.YCoord == 6 &&
                                                                MockupVertices[2, 3].NeighbourUpper.YCoord == 7 &&
                                                                MockupVertices[2, 3].NeighbourUpperRight == null &&
                                                                MockupVertices[2, 3].NeighbourRight == null &&
                                                                MockupVertices[2, 3].NeighbourLowerRight == null &&
                                                                MockupVertices[2, 3].NeighbourLower.YCoord == 15 &&
                                                                MockupVertices[2, 3].NeighbourLowerLeft.YCoord == 14
                                                                )
                                                    {
                                                        //12
                                                        if
                                                                    (
                                                                    MockupVertices[3, 0].NeighbourLeft == null &&
                                                                    MockupVertices[3, 0].NeighbourUpperLeft == null &&
                                                                    MockupVertices[3, 0].NeighbourUpper.YCoord == 8 &&
                                                                    MockupVertices[3, 0].NeighbourUpperRight.YCoord == 9 &&
                                                                    MockupVertices[3, 0].NeighbourRight.YCoord == 13 &&
                                                                    MockupVertices[3, 0].NeighbourLowerRight == null &&
                                                                    MockupVertices[3, 0].NeighbourLower == null &&
                                                                    MockupVertices[3, 0].NeighbourLowerLeft == null
                                                                    )
                                                        {
                                                            //13
                                                            if
                                                                        (
                                                                        MockupVertices[3, 1].NeighbourLeft.YCoord == 12 &&
                                                                        MockupVertices[3, 1].NeighbourUpperLeft.YCoord == 8 &&
                                                                        MockupVertices[3, 1].NeighbourUpper.YCoord == 9 &&
                                                                        MockupVertices[3, 1].NeighbourUpperRight.YCoord == 10 &&
                                                                        MockupVertices[3, 1].NeighbourRight.YCoord == 14 &&
                                                                        MockupVertices[3, 1].NeighbourLowerRight == null &&
                                                                        MockupVertices[3, 1].NeighbourLower == null &&
                                                                        MockupVertices[3, 1].NeighbourLowerLeft == null
                                                                        )
                                                            {
                                                                //14
                                                                if
                                                                            (
                                                                            MockupVertices[3, 2].NeighbourLeft.YCoord == 13 &&
                                                                            MockupVertices[3, 2].NeighbourUpperLeft.YCoord == 8 &&
                                                                            MockupVertices[3, 2].NeighbourUpper.YCoord == 9 &&
                                                                            MockupVertices[3, 2].NeighbourUpperRight.YCoord == 10 &&
                                                                            MockupVertices[3, 2].NeighbourRight.YCoord == 15 &&
                                                                            MockupVertices[3, 2].NeighbourLowerRight == null &&
                                                                            MockupVertices[3, 2].NeighbourLower == null &&
                                                                            MockupVertices[3, 2].NeighbourLowerLeft == null
                                                                            )
                                                                {
                                                                    //15
                                                                    if
                                                                                (
                                                                                MockupVertices[3, 3].NeighbourLeft.YCoord == 14 &&
                                                                                MockupVertices[3, 3].NeighbourUpperLeft.YCoord == 10 &&
                                                                                MockupVertices[3, 3].NeighbourUpper.YCoord == 11 &&
                                                                                MockupVertices[3, 3].NeighbourUpperRight == null &&
                                                                                MockupVertices[3, 3].NeighbourRight == null &&
                                                                                MockupVertices[3, 3].NeighbourLowerRight == null &&
                                                                                MockupVertices[3, 3].NeighbourLower == null &&
                                                                                MockupVertices[3, 3].NeighbourLowerLeft == null
                                                                                )
                                                                    {
                                                                        FoundCorrectNeighbours = true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        Assert.IsTrue(FoundCorrectNeighbours);
    }
}
