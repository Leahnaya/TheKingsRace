using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : NetworkBehaviour
{
    private Rigidbody boulder;
    public float bumpPower = 70;

    void Start() {
       boulder = GetComponent<Rigidbody>();//Gets the rigidbody attached to the bolder
    }

    void OnTriggerEnter(Collider objectHit) {
        if (objectHit.gameObject.tag == "ArcherTarget") {
                    
            Debug.Log("Bump");
            MoveStateManager playerMovement = objectHit.GetComponent<MoveStateManager>();

            Vector3 dirBump = objectHit.transform.position - transform.position;

            dirBump.y = .1f;
            if(dirBump.x == 0 && dirBump.z == 0){
                dirBump = new Vector3(1,.1f,1);
            }

            playerMovement.GetHit(dirBump.normalized, bumpPower);
        }
    }

    public void StartCountdown(int time, Vector3 spawnForce)
    {
        StartCoroutine(DespawnCounter(time));
        AddForceServerRPC(spawnForce);
    }

    [ServerRpc]
    private void AddForceServerRPC(Vector3 force) {
        this.gameObject.GetComponent<Rigidbody>().AddForce(force);
    }

    IEnumerator DespawnCounter(int time) {
        for (int i = time; i > 0; i--) {
            yield return new WaitForSecondsRealtime(1f);
        }

        // Time's up - Despawn us
        DespawnBoulderServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnBoulderServerRPC() {
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
