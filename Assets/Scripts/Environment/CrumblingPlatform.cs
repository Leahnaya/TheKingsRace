using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblingPlatform : MonoBehaviour
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
        mesh.enabled = false;
        boxColliders[0].enabled = false;
        boxColliders[1].enabled = false;
        
        //Debug.Log("Crumbled");
        yield return new WaitForSecondsRealtime(2.0f);
        mesh.enabled = true;
        boxColliders[0].enabled = true;
        boxColliders[1].enabled = true;

        //Debug.Log("Respawned Platform");
        cooldown = false;
    }
}
