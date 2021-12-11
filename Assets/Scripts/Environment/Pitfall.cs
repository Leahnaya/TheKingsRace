using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : NetworkBehaviour
{
    [SerializeField] private Vector3 RespawnPoint; //Where player will respawn to, set in GUI

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject.tag == "Player" && other.gameObject.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            RespawnPlayerServerRPC();
        }
    }

    [ServerRpc]
    private void RespawnPlayerServerRPC(ServerRpcParams serverRpcParams = default) {
        RespawnPlayerClientRPC(serverRpcParams.Receive.SenderClientId);
    }

    [ClientRpc]
    private void RespawnPlayerClientRPC(ulong playerID) {
        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Find our player first
        foreach (GameObject character in playableCharacters)
        {
            if (character.GetComponent<NetworkObject>().OwnerClientId == playerID)
            {
                character.GetComponentInChildren<PlayerMovement>().CancelMomentum();
                Debug.Log(character.transform.position);
                character.transform.position = RespawnPoint;
                Debug.Log(character.transform.position);
            }
        }
    }
}
