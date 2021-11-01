using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCam : MonoBehaviour
{
    public GameObject player;

    private Vector3 offset;
    private Vector3 rad;

    void Start ()
    {
        rad = (transform.position - player.transform.position);
    }

    void Update ()
    {
        offset = transform.parent.forward * rad.magnitude;
        transform.position = new Vector3((player.transform.position.x - offset.x),((player.transform.position.y - offset.y)+2),(player.transform.position.z - offset.z));
        
    }
}
