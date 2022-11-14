using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    #region Tile Import
    public GameObject GrassTile;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        Instantiate(GrassTile, new Vector3(0, 0, 0), Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
