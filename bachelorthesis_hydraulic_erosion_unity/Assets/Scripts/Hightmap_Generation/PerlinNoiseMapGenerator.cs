using System;
using UnityEngine;
using UnityEngine.UI;

public class PerlinNoiseMapGenerator : MonoBehaviour
{
    public string PXwidth { get; set; }
    public string PYheight { get; set; }

    public string POffsetX { get; set; }
    public string POffsetY { get; set; }

    public string PStrength { get; set; }
    public string PScale { get; set; }

    private int Xwidth { get; set;}
    private int Yheight { get; set; }

    private float OffsetX { get; set; }
    private float OffsetY { get; set; }

    private float Strength { get; set; }
    private float Scale { get; set; }


    public Image image;

    private void CastAll()
    {
        Xwidth = Int32.Parse(PXwidth);
        Yheight = Int32.Parse(PYheight);

        OffsetX = float.Parse(POffsetX);
        OffsetY = float.Parse(POffsetY);

        Strength = float.Parse(PStrength);
        Scale = float.Parse(PScale);
}

    public void GenerateTexture()
    {
        CastAll();

        Debug.Log("Xwidth: " + Xwidth + ", Yheight: " + Yheight +", offsetX: " + OffsetX + ", offsetY: " + OffsetY + ", Strength: " + Strength + ", Scale: " + Scale);

        Texture2D texture = new Texture2D(Xwidth, Yheight);

        for(int x = 0; x< Xwidth; x++)
        {
            for (int y = 0; y < Yheight; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = (float) x / Xwidth * Scale + OffsetX;
        float yCoord = (float) y / Yheight * Scale + OffsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord) * Strength;
        return new Color(sample, sample, sample);
    }
}
