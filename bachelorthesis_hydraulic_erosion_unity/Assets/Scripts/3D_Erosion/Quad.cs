public class Quad
{
    public Vertex TopLeftVertex;
    public Vertex TopRightVertex;
    public Vertex BottomLeftVertex;
    public Vertex BottomRightVertex;

    public Quad(Vertex TopLeft, Vertex TopReight, Vertex BottomRight, Vertex BottomLeft)
    {
        TopLeftVertex = TopLeft;
        TopRightVertex = TopReight;
        BottomLeftVertex = BottomLeft;
        BottomRightVertex = BottomRight;
    }

    public void RenderQuad()
    {
        //TODO: Render Quad
    }
}
