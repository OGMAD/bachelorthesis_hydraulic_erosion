using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Embet : MonoBehaviour
{
    private float[,] PathTraces;
    public Embet(float[,] PathTracesGet)
    {
        PathTraces = PathTracesGet;
    }
    public void EmbedErosionInLandscape(Vertex LastVertex, Vertex CurrentVertex)
    {
        if (CurrentVertex.ReturnFalseIfSomeNeighbourIsNull())
        {
            (Vertex, Vertex) LeftAndRightNeighbour = CurrentVertex.GetLeftAndRightNeighbourBasedOnDirections(LastVertex);

            Vertex LeftNeighbour = LeftAndRightNeighbour.Item1;
            PathTraces[LeftNeighbour.XCoord, LeftNeighbour.ZCoord] = 0.75f;
            Vertex LeftNeighbourPlusOne = null;
            Vertex LeftNeighbourPlusTwo = null;

            Vertex RightNeighbour = LeftAndRightNeighbour.Item2;
            PathTraces[RightNeighbour.XCoord, RightNeighbour.ZCoord] = 0.75f;
            Vertex RightNeighbourPlusOne = null;
            Vertex RightNeighbourPlusTwo = null;

            if (LeftNeighbour.ReturnFalseIfSomeNeighbourIsNull())
            {
                LeftNeighbourPlusOne = LeftNeighbour.GetNextVertexBasedOnDirection(CurrentVertex);
                PathTraces[LeftNeighbourPlusOne.XCoord, LeftNeighbourPlusOne.ZCoord] = 0.5f;

                if (LeftNeighbourPlusOne.ReturnFalseIfSomeNeighbourIsNull())
                {
                    LeftNeighbourPlusTwo = LeftNeighbourPlusOne.GetNextVertexBasedOnDirection(LeftNeighbour);
                    PathTraces[LeftNeighbourPlusTwo.XCoord, LeftNeighbourPlusTwo.ZCoord] = 0.25f;
                }
            }

            if (RightNeighbour.ReturnFalseIfSomeNeighbourIsNull())
            {
                RightNeighbourPlusOne = RightNeighbour.GetNextVertexBasedOnDirection(CurrentVertex);
                PathTraces[RightNeighbourPlusOne.XCoord, RightNeighbourPlusOne.ZCoord] = 0.5f;

                if (RightNeighbourPlusOne.ReturnFalseIfSomeNeighbourIsNull())
                {
                    RightNeighbourPlusTwo = RightNeighbourPlusOne.GetNextVertexBasedOnDirection(RightNeighbour);
                    PathTraces[RightNeighbourPlusTwo.XCoord, RightNeighbourPlusTwo.ZCoord] = 0.25f;
                }
            }

            (List<Vertex>, int) PathScliceAndCenter = TurnIntoListAndCenter(LeftNeighbour, LeftNeighbourPlusOne, LeftNeighbourPlusTwo, CurrentVertex, RightNeighbour, RightNeighbourPlusOne, RightNeighbourPlusTwo);
            DistributefromCenterToSides(PathScliceAndCenter.Item1, PathScliceAndCenter.Item2);

        }


    }
    public (List<Vertex>, int) TurnIntoListAndCenter
        (
        Vertex LeftNeighbour,
        Vertex LeftNeighbourPlusOne,
        Vertex LeftNeighbourPlusTwo,
        Vertex CurrentVertex,
        Vertex RightNeighbour,
        Vertex RightNeighbourPlusOne,
        Vertex RightNeighbourPlusTwo)
    {
        int Center;
        List<Vertex> PathSlice = new();
        if (LeftNeighbourPlusTwo != null)
        {
            PathSlice.Add(LeftNeighbourPlusTwo);
        }

        if (LeftNeighbourPlusOne != null)
        {
            PathSlice.Add(LeftNeighbourPlusOne);
        }

        if (LeftNeighbour != null)
        {
            PathSlice.Add(LeftNeighbour);
        }

        PathSlice.Add(CurrentVertex);
        Center = PathSlice.Count - 1;

        if (RightNeighbour != null)
        {
            PathSlice.Add(RightNeighbour);
        }

        if (RightNeighbourPlusOne != null)
        {
            PathSlice.Add(RightNeighbourPlusOne);
        }

        if (RightNeighbourPlusTwo != null)
        {
            PathSlice.Add(RightNeighbourPlusTwo);
        }

        return (PathSlice, Center);
    }

    public void DistributefromCenterToSides(List<Vertex> PathSclice, int ScliceCenter)
    {
        //TODO:Implement
    }
}
