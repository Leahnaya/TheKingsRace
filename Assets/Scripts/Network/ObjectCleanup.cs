using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class ObjectCleanup : NetworkBehaviour {
    
    void Start() {
        DontDestroyOnLoad(gameObject);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DestroyPersistingNetObjectsServerRPC() {
        NetworkObject[] networkObjects = FindObjectsOfType<NetworkObject>();
        Debug.Log("Here");
        foreach (NetworkObject nObj in networkObjects) {
            GameObject netObj = nObj.gameObject;

            if (netObj == gameObject) { continue; }

            netObj.GetComponent<NetworkObject>().Despawn(true);
        }
    }
}
