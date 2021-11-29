using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    Vector3 rotation = new Vector3(0.0f, .75f, 0.0f);
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        this.gameObject.transform.Rotate(rotation);
    }
}
