using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    public GameObject Content;
    public GameObject CloudmapItem_prefab;
    private List<CloudmapItem> CloudmapItem_List = new List<CloudmapItem>();

    #region Variables
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
        CloudmapItem item = new CloudmapItem();
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
        Debug.Log("SetValuesInEdit");
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

    public void GeneratePerlinNoiseMap()
    {
        CastAll();
        CloudmapItem item = GetActiveCloudmapItem();
        item.Name = Name;
        item.Xwidth = Xwidth;
        item.Yheight = Yheight;
        item.OffsetX = OffsetX;
        item.OffsetY = OffsetY;
        item.Strength = Strength;
        item.Scale = Scale;
    }
}

public class CloudmapItem
{
    public GameObject obj { get; set; }
    public string Name { get; set; }
    public int Xwidth { get; set; }
    public int Yheight { get; set; }

    public float OffsetX { get; set; }
    public float OffsetY { get; set; }

    public float Strength { get; set; }
    public float Scale { get; set; }

    public bool IsActive { get; set; }

    public CloudmapItem()
    {
        Name = "SomeHightmap";
        Xwidth = 255;
        Yheight = 255;
        OffsetX = 0;
        OffsetY = 0;
        Strength = 1;
        Scale = 4;
        IsActive = false;
    }
}
