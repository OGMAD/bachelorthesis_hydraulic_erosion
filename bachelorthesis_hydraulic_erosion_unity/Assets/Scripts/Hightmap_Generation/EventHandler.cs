using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandler : MonoBehaviour
{
    public GameObject Content;
    public GameObject CloudmapItem_prefab;

    public void NewCloudmapItem()
    {
        GameObject CloudmapItem = Instantiate(CloudmapItem_prefab);
        CloudmapItem.transform.SetParent(Content.transform);
        CloudmapItem.transform.localScale = new Vector3(1,1,1);
    }
}
