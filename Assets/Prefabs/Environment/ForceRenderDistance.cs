using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceRenderDistance : MonoBehaviour
{
    [SerializeField]
    private int detailRenderDistance = 1000;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.GetComponent<Terrain>().detailObjectDistance = detailRenderDistance;
    }

}
