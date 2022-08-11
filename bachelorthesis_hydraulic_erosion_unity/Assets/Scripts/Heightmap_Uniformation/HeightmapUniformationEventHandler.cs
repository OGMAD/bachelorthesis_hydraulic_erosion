using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeightmapUniformationEventHandler : MonoBehaviour
{
    public static int PixelPerKilometer = 1000;
    public string XWidthKMString { get; set; }
    public string YHeightKMString { get; set; }
    public Image HeightMapImage;
    private Sprite SourceHeightMapSprite;
    private Sprite TargetSprite;
    private float XWidthKM;
    private float YHeightKM;
    private int SourceXWidthPX;
    private int SourceYHeightPX;
    private int TargetXWidthPX;
    private int TargetYHeightPX;
    private float ScaleFactorX;
    private float ScaleFactorY;


    void Start()
    {
        SourceHeightMapSprite = StateNameController.HeightMapSprite;
        SourceXWidthPX = SourceHeightMapSprite.texture.width;
        SourceYHeightPX = SourceHeightMapSprite.texture.height;
    }

    public void Unificate()
    {
        CalculateTargetDimensions();
        CalculateScaleFactors();
        Texture2D texture = new Texture2D(TargetXWidthPX, TargetYHeightPX);
        TargetSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        CalculateTargetTexture();
    }
    private void CalculateTargetDimensions()
    {
        XWidthKM = float.Parse(XWidthKMString);
        YHeightKM = float.Parse(YHeightKMString);

        TargetXWidthPX = (int)(PixelPerKilometer * XWidthKM);
        TargetYHeightPX = (int)(PixelPerKilometer * YHeightKM);
    }
    private void CalculateScaleFactors()
    {
        ScaleFactorX = (float)TargetXWidthPX / (float)SourceXWidthPX;
        ScaleFactorY = (float)TargetYHeightPX / (float)SourceYHeightPX;
    }
    private void CalculateTargetTexture()
    {


        if (ScaleFactorX < 1 && ScaleFactorY < 1)
        {
            ScaleDown();
        } else if(ScaleFactorX > 1 && ScaleFactorY > 1)
        {
            ScaleUp();
        }
        else
        {
            MixedScaling();
        }

        TargetSprite.texture.Apply();
        HeightMapImage.sprite = TargetSprite;
    }

    private void ScaleUp()
    {
        //kinda works
        for(int x = 0; x < TargetXWidthPX; x++)
        {
            for(int y = 0; y < TargetYHeightPX; y++)
            {
                TargetSprite.texture.SetPixel(x, y, SourceHeightMapSprite.texture.GetPixel((int)(x / ScaleFactorX), (int)(y / ScaleFactorY)));
            }
        }
    }
    private void ScaleDown()
    {
        for (int x = 0; x < TargetXWidthPX; x++)
        {
            for (int y = 0; y < TargetYHeightPX; y++)
            {
                TargetSprite.texture.SetPixel(x, y, ScaleDownPixelgroupFor(x,y));
            }
        }
    }

    private Color ScaleDownPixelgroupFor(int x, int y)
    {
        float xa = x / ScaleFactorX;
        float xb = xa + (1 / ScaleFactorX);
        float ya = y / ScaleFactorY;
        float yb = ya + (1 / ScaleFactorY);

        float r = 0.0f;
        float g = 0.0f;
        float b = 0.0f;

        int count = 0;

        for(int xi = (int) xa; xi <= (int) xb; xi++)
        {
            for(int yi = (int) ya; yi <= (int) yb; yi++)
            {
                r += SourceHeightMapSprite.texture.GetPixel(xi, yi).r;
                g += SourceHeightMapSprite.texture.GetPixel(xi, yi).g;
                b += SourceHeightMapSprite.texture.GetPixel(xi, yi).b;
                count++;
            }
        }
        Debug.Log("----------------");
        return new Color(r / count, g / count, b / count);
    }

    private void MixedScaling()
    {

    }
}
