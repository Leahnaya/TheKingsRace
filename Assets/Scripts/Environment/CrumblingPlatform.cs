using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class CrumblingPlatform : NetworkBehaviour
{
    private bool cooldown = false;
    private MeshRenderer mesh;
    private BoxCollider[] boxColliders;

    private void Start()
    {
        mesh = gameObject.GetComponent<MeshRenderer>();
        boxColliders = gameObject.GetComponents<BoxCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collision detected");
        if (other.CompareTag("ArcherTarget") && cooldown == false)
        {
            //Debug.Log("Collided with player");
            cooldown = true;
            StartCoroutine(DeleteCooldown());
        }
    }

   

    IEnumerator DeleteCooldown()
    {
        //Debug.Log("Crumbling");
        yield return new WaitForSecondsRealtime(.25f);
        ChangeCrublingPlatformsStateServerRPC(false);
        
        //Debug.Log("Crumbled");
        yield return new WaitForSecondsRealtime(2.0f);
        ChangeCrublingPlatformsStateServerRPC(true);

        //Debug.Log("Respawned Platform");
        cooldown = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void ChangeCrublingPlatformsStateServerRPC(bool active) {
        ChangeStateClientRPC(active);
    }

    [ClientRpc]
    private void ChangeStateClientRPC(bool active) {
        mesh.enabled = active;
        boxColliders[0].enabled = active;
        boxColliders[1].enabled = active;
    }
}
