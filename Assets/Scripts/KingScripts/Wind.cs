using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector3 DirWind;
    // Start is called before the first frame update
    void Start()
    {
        DirWind = Vector3.forward;
    }

    // Update is called once per frame
    void OnTriggerStay(Collider other)
    {
        other.GetComponent<AerialStateManager>().AddImpact(DirWind, 1);
    }
}
