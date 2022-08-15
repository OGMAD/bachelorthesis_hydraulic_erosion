using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeDErosionEventHandler : MonoBehaviour
{
    private Sprite HeightMap;

    // Start is called before the first frame update
    void Start()
    {
        //GetAndSetHeightMap();
        //Debug.Log(HeightMap.texture.GetPixel(100, 100));
        MeshGenerationHelper.CreatePlane(5, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void GetAndSetHeightMap()
    {
        HeightMap = StateNameController.HeightMapSpriteUniformated;
    }
}
