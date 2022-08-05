using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    public GameObject Content;
    public GameObject CloudmapItem_prefab;
    private List<GameObject> CloudmapItem_List = new List<GameObject>();

    public string PName { get; set; }
    public string Name { get; set; }

    public string PXwidth { get; set; }
    public string PYheight { get; set; }

    public string POffsetX { get; set; }
    public string POffsetY { get; set; }

    public string PStrength { get; set; }
    public string PScale { get; set; }

    private int Xwidth { get; set; }
    private int Yheight { get; set; }

    private float OffsetX { get; set; }
    private float OffsetY { get; set; }

    private float Strength { get; set; }
    private float Scale { get; set; }

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
        CloudmapItem.GetComponent<Button>().onClick.AddListener(() => ChangeColor(CloudmapItem));
        CloudmapItem_List.Add(CloudmapItem);
    }

    //public void Update()
    //{
    //    Debug.Log("Update");
    //    for (int x = 0; x < CloudmapItem_List.Count; x++)
    //    {
    //        CloudmapItem_List[x].GetComponent<Button>().onClick.AddListener(() => ChangeColor(CloudmapItem_List[x], x));
    //    }
    //}

    private void ChangeColor(GameObject obj)
    {
        System.Random random = new System.Random();
        byte a = (byte)random.Next(256);
        byte b = (byte)random.Next(256);
        byte g = (byte)random.Next(256);
        byte r = (byte)random.Next(256);

        Debug.Log("(" + a + ", " + b + ", " + g + ", " + r + ")");
        obj.GetComponent<Image>().color = new Color32(a, b, g, r);
    }
}
