using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.Threading;
using System.Threading.Tasks;

public class HightmapGenerationEventHandler : MonoBehaviour
{
    #region Variables
    public GameObject Content;
    public GameObject CloudmapItem_prefab;
    private List<CloudmapItem> CloudmapItem_List = new List<CloudmapItem>();
    public string PName { get; set; }
    public InputField GName;
    
    public string PXwidth { get; set; }
    public InputField GXwidth;
    public string PYheight { get; set; }
    public InputField GYheight;
    public string POffsetX { get; set; }
    public InputField GOffsetX;
    public string POffsetY { get; set; }
    public InputField GOffsertY;

    public string PStrength { get; set; }
    public InputField GStrength;
    public string PScale { get; set; }
    public InputField GScale;


    private string Name { get; set; }
    private int Xwidth { get; set; }
    private int Yheight { get; set; }

    private float OffsetX { get; set; }
    private float OffsetY { get; set; }

    private float Strength { get; set; }
    private float Scale { get; set; }

    public Image Selected_img;
    public Image Overall_img;

    public Slider ProgressBar;
    private float Progress;
    #endregion

    private void CastAll()
    {
        Name = PName;

        Xwidth = System.Int32.Parse(PXwidth);
        Yheight = System.Int32.Parse(PYheight);

        OffsetX = float.Parse(POffsetX);
        OffsetY = float.Parse(POffsetY);

        Strength = float.Parse(PStrength);
        Scale = float.Parse(PScale);
    }

    public void NewCloudmapItem()
    {
        GameObject CloudmapItem = Instantiate(CloudmapItem_prefab);
        CloudmapItem.transform.SetParent(Content.transform);
        CloudmapItem.transform.localScale = new Vector3(1,1,1);
        CloudmapItem.GetComponent<Button>().onClick.AddListener(() => SetAsActive(CloudmapItem));
        CloudmapItem item = new CloudmapItem(Selected_img);
        item.obj = CloudmapItem;
        CloudmapItem_List.Add(item);
    }

    private void SetAsActive(GameObject obj)
    {
        for(int x = 0; x < CloudmapItem_List.Count; x++)
        {
            CloudmapItem_List[x].obj.gameObject.GetComponent<Image>().color = new Color32(255, 255, 255, 255);
        }

        obj.GetComponent<Image>().color = new Color32(255, 215, 210, 255);

        for (int x = 0; x < CloudmapItem_List.Count; x++)
        {
            if (CloudmapItem_List[x].obj.gameObject.GetComponent<Image>().color == new Color32(255, 255, 255, 255))
            {
                CloudmapItem_List[x].IsActive = false;
            }
            else
            {
                CloudmapItem_List[x].IsActive = true;
                SetValuesInEdit(CloudmapItem_List[x]);
            }
        }
    }

    private void SetValuesInEdit(CloudmapItem item)
    {
        GName.text = item.Name;
        GXwidth.text = item.Xwidth.ToString();
        GYheight.text = item.Yheight.ToString();
        GOffsetX.text = item.OffsetX.ToString();
        GOffsertY.text = item.OffsetY.ToString();
        GStrength.text = item.Strength.ToString();
        GScale.text = item.Scale.ToString();
        
        Selected_img.sprite = item.obj_img.sprite;
        
    }

    private CloudmapItem GetActiveCloudmapItem()
    {
        for (int x = 0; x < CloudmapItem_List.Count; x++)
        {
            if (CloudmapItem_List[x].IsActive)
            {
                return CloudmapItem_List[x];
            }
        }
        return null;
    }

    private void SetDimantionsForAll()
    {
        bool Regenerate = false;
        for(int i = 0; i < CloudmapItem_List.Count; i++)
        {
            if(CloudmapItem_List[i].Xwidth != Xwidth || CloudmapItem_List[i].Yheight != Yheight)
            {
                Regenerate = true;
            }
            CloudmapItem_List[i].Xwidth = Xwidth;
            CloudmapItem_List[i].Yheight = Yheight;
        }
        if (Regenerate)
        {
            RegenerateHeightmaps();
        }
    }

    private void RegenerateHeightmaps()
    {
        for (int i = 0; i < CloudmapItem_List.Count; i++)
        {
            CloudmapItem_List[i].GeneratePerlinNoise();
        }
    }

    public void GeneratePerlinNoiseMap()
    {
        CastAll();
        SetDimantionsForAll();
        CloudmapItem item = GetActiveCloudmapItem();
        item.Name = Name;
        item.Xwidth = Xwidth;
        item.Yheight = Yheight;
        item.OffsetX = OffsetX;
        item.OffsetY = OffsetY;
        item.Strength = Strength;
        item.Scale = Scale;

        item.SetNameAndImg();
        item.GeneratePerlinNoise();
        Selected_img.sprite = item.obj_img.sprite;

        GenerateOverallResault();
    }

    private void GenerateOverallResault()
    {
        Progress = 0.0f;
        ProgressBar.value = Progress;
        Texture2D texture = new Texture2D(Xwidth, Yheight);
        Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

        for (int x = 1; x < CloudmapItem_List.Count; x++)
        {
            sprite = BlendSpritesTogether(CloudmapItem_List[x-1].obj_img.sprite, CloudmapItem_List[x].obj_img.sprite);
        }
        Overall_img.sprite = sprite;
    }

    private Sprite BlendSpritesTogether(Sprite SpriteA, Sprite SpriteB)
    {
        Texture2D textureA = SpriteA.texture;
        Texture2D textureB = SpriteB.texture;
        Texture2D textureC = new Texture2D(Xwidth, Yheight);
        
        for (int x = 0; x< textureA.width; x++)
        {
            for(int y = 0; y< textureA.height;y++)
            {
                Color color = (textureA.GetPixel(x, y) * textureB.GetPixel(x, y));
                textureC.SetPixel(x, y, color);
            }
        }
        textureC.Apply();
        return Sprite.Create(textureC, new Rect(0, 0, textureC.width, textureC.height), Vector2.zero);
    }

    private string path;
    public void LoadHeightmap()
    {
        path = EditorUtility.OpenFilePanel("Overwrite with png", "", "png");
        if(path != null)
        {
            WWW www = new WWW("file:///" + path);
            StateNameController.HeightMapSprite = Sprite.Create(www.texture, new Rect(0, 0, www.texture.width, www.texture.height), Vector2.zero);
        }
        SceneManager.LoadScene("Heightmap_Uniformation");
    }

    public void Submit()
    {
        StateNameController.HeightMapSprite = Overall_img.sprite;
        SceneManager.LoadScene("Heightmap_Uniformation");
    }
}

public class CloudmapItem
{
    public GameObject obj { get; set; }
    public Text obj_name { get; set; }
    public Image obj_img { get; set; }
    public string Name { get; set; }
    public int Xwidth { get; set; }
    public int Yheight { get; set; }

    public float OffsetX { get; set; }
    public float OffsetY { get; set; }

    public float Strength { get; set; }
    public float Scale { get; set; }

    public bool IsActive { get; set; }

    public CloudmapItem(Image image)
    {
        Name = "SomeHightmap";
        Xwidth = 255;
        Yheight = 255;
        OffsetX = 0;
        OffsetY = 0;
        Strength = 1;
        Scale = 4;
        IsActive = false;

        obj_img = image;
    }

    public void SetNameAndImg()
    {
        obj_name = obj.GetComponentsInChildren<Text>()[0];
        obj_img = obj.GetComponentsInChildren<Image>()[1];

        obj_name.text = Name;
    }

    public void GeneratePerlinNoise()
    {
        Texture2D texture = new Texture2D(Xwidth, Yheight);

        for (int x = 0; x < Xwidth; x++)
        {
            for (int y = 0; y < Yheight; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        obj_img.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }

    Color CalculateColor(int x, int y)
    {
        float xCoord = (float)x / Xwidth * Scale + OffsetX;
        float yCoord = (float)y / Yheight * Scale + OffsetY;

        float sample = Mathf.PerlinNoise(xCoord, yCoord)+(Strength/100);
        return new Color(sample, sample, sample);
    }
}
