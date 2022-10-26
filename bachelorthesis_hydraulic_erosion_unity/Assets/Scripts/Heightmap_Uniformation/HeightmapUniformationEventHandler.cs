using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HeightmapUniformationEventHandler : MonoBehaviour
{
    public static int PixelPerKilometer = 1000;
    public string XWidthKMString { get; set; }
    public string YHeightKMString { get; set; }
    public string HighestPointMString { get; set; }
    public string LowestPointMString { get; set; }

    public Image HeightMapImage;
    private Sprite SourceHeightMapSprite;
    private Sprite TargetSpriteX;
    private Sprite TargetSpriteXY;
    private float XWidthKM;
    private float YHeightKM;
    private int SourceXWidthPX;
    private int SourceYHeightPX;
    private int TargetXWidthPX;
    private int TargetYHeightPX;
    private float ScaleFactorX;
    private float ScaleFactorY;

    public Slider ProgressBar;
    private float Progress;


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

        Texture2D textureX = new Texture2D(TargetXWidthPX, SourceYHeightPX);
        TargetSpriteX = Sprite.Create(textureX, new Rect(0, 0, textureX.width, textureX.height), Vector2.zero);

        Texture2D textureXY = new Texture2D(TargetXWidthPX, TargetYHeightPX);
        TargetSpriteXY = Sprite.Create(textureXY, new Rect(0, 0, textureXY.width, textureXY.height), Vector2.zero);

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

        TargetSpriteXY.texture.Apply();
        HeightMapImage.sprite = TargetSpriteXY;
    }

    private void ScaleUp()
    {
        for(int x = 0; x < TargetXWidthPX; x++)
        {
            for(int y = 0; y < TargetYHeightPX; y++)
            {
                TargetSpriteXY.texture.SetPixel(x, y, SourceHeightMapSprite.texture.GetPixel((int)(x / ScaleFactorX), (int)(y / ScaleFactorY)));
            }
        }
    }
    private void ScaleDown()
    {
        for (int x = 0; x < TargetXWidthPX; x++)
        {
            for (int y = 0; y < TargetYHeightPX; y++)
            {
                TargetSpriteXY.texture.SetPixel(x, y, ScaleDownPixelgroupFor(SourceHeightMapSprite,x, y));
            }
        }
    }

    private Color ScaleDownPixelgroupFor(Sprite sprite, int x, int y)
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
                r += sprite.texture.GetPixel(xi, yi).r;
                g += sprite.texture.GetPixel(xi, yi).g;
                b += sprite.texture.GetPixel(xi, yi).b;
                count++;
            }
        }
        return new Color(r / count, g / count, b / count);
    }

    private void MixedScaling()
    {
        ScaleOnX();
        ScaleOnY();
        TargetSpriteXY.texture.Apply();
    }

    private void ScaleOnX()
    {
        char Axis = 'x';

        if(ScaleFactorX == 1)
        {
            ConfirmScaleOnSingleAxis(Axis);
        } else if(ScaleFactorX < 1)
        {
            DownscaleOnSingleAxis(Axis);
        } else if(ScaleFactorX > 1)
        {
            UpscaleOnSingleAxis(Axis);
        }
    }

    private void ScaleOnY()
    {
        char Axis = 'y';

        if (ScaleFactorY == 1)
        {
            ConfirmScaleOnSingleAxis(Axis);
        }
        else if (ScaleFactorY < 1)
        {
            DownscaleOnSingleAxis(Axis);
        }
        else if (ScaleFactorY > 1)
        {
            UpscaleOnSingleAxis(Axis);
        }
    }

    private void UpscaleOnSingleAxis(char Axis)
    {
        switch (Axis)
        {
            case 'x':
                for (int x = 0; x < TargetXWidthPX; x++)
                {
                    for (int y = 0; y < SourceYHeightPX; y++)
                    {
                        TargetSpriteX.texture.SetPixel(x, y, SourceHeightMapSprite.texture.GetPixel((int)(x / ScaleFactorX), y));
                    }
                }
                break;
            case 'y':
                for (int x = 0; x < TargetXWidthPX; x++)
                {
                    for (int y = 0; y < TargetYHeightPX; y++)
                    {
                        TargetSpriteXY.texture.SetPixel(x, y, TargetSpriteX.texture.GetPixel(x, (int)(y / ScaleFactorY)));
                    }
                }
                break;
        }
    }

    private void DownscaleOnSingleAxis(char Axis)
    {
        switch (Axis)
        {
            case 'x':
                for (int x = 0; x < TargetXWidthPX; x++)
                {
                    for (int y = 0; y < SourceYHeightPX; y++)
                    {
                        TargetSpriteX.texture.SetPixel(x, y, ScaleDownPixelgroupFor(SourceHeightMapSprite, x, y));
                    }
                }
                break;
            case 'y':
                for (int x = 0; x < TargetXWidthPX; x++)
                {
                    for (int y = 0; y < TargetYHeightPX; y++)
                    {
                        TargetSpriteXY.texture.SetPixel(x, y, ScaleDownPixelgroupFor(TargetSpriteX, x, y));
                    }
                }
                break;
        }
    }

    private void ConfirmScaleOnSingleAxis(char Axis)
    {
        switch (Axis)
        {
            case 'x':
                for (int x = 0; x < TargetXWidthPX; x++)
                {
                    for (int y = 0; y < SourceYHeightPX; y++)
                    {
                        TargetSpriteX.texture.SetPixel(x, y, SourceHeightMapSprite.texture.GetPixel(x,y));
                    }
                }
                break;
            case 'y':
                for (int x = 0; x < TargetXWidthPX; x++)
                {
                    for (int y = 0; y < TargetYHeightPX; y++)
                    {
                        TargetSpriteXY.texture.SetPixel(x, y, TargetSpriteX.texture.GetPixel(x, y));
                    }
                }
                break;
        }
    }

    public void Submit()
    {
        StateNameController.HeightMapSpriteUniformated = TargetSpriteXY;
        StateNameController.HighestPoint = int.Parse(HighestPointMString);
        StateNameController.LowestPoint = int.Parse(LowestPointMString);
        //SceneManager.LoadScene("3D_Erosion");
        SceneManager.LoadScene("displacement_map_test");
    }
}
