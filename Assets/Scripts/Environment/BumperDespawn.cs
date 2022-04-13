using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class BumperDespawn : NetworkBehaviour {

    private float despawnTimer = 20f;

    // Start is called before the first frame update
    void Start() {
        // Only start the despawn timer on the host system
        if (!IsHost) { return; }

        // Start the Despawn Timer
        StartCoroutine(DespawnTimer());
    }

    IEnumerator DespawnTimer() {
        // Wait for the timer to complete
        yield return new WaitForSecondsRealtime(despawnTimer);

        // Despawn the Bumper this is attached to
        DespawnMyselfServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnMyselfServerRPC() {
        gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
