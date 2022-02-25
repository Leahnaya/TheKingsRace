using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class GateDespawner : NetworkBehaviour {

    private float despawnTime = 10f;

    void Update() { 
        // Check if the door has been kicked
        // Object won't be kinematic anymore if kicked
        if (!this.gameObject.GetComponent<Rigidbody>().isKinematic) { 
            // Only have the host start the timer
            if (IsHost) {
                StartCoroutine(DespawnTimer());
                Physics.IgnoreLayerCollision(13,14);
            }
        }
    }

    IEnumerator DespawnTimer() {
        // Wait for set time
        yield return new WaitForSecondsRealtime(despawnTime);

        // Despawn the door
        DespawnGateServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnGateServerRPC() {
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }

}
