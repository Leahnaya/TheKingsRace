using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : MonoBehaviour
{
    public Vector3 RespawnPoint; //Where player will respawn to, set in GUI

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) //If runner enters the trigger cancel momentum, disable controls, and move to set respawn point
        {
            other.transform.position = RespawnPoint;
            other.GetComponent<PlayerMovement>().CancelMomentum();
        }
    }
}
