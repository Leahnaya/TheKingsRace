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

    [SerializeField]
    public float crumbleTime = 0.25f;
    [SerializeField]
    public float respawnTime = 2.0f;

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
        yield return new WaitForSecondsRealtime(crumbleTime);
        ChangeCrublingPlatformsStateServerRPC(false);
        
        //Debug.Log("Crumbled");
        yield return new WaitForSecondsRealtime(respawnTime);
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
