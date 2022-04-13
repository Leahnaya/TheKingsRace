using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class SlowTerrain : NetworkBehaviour
{
    void OnTriggerEnter(Collider other) {
        if (other.transform.gameObject.tag == "PlayerTrigger" && other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            ApplySlowdownServerRPC(other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplySlowdownServerRPC(ulong playerID) {
        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { playerID }
            }
        };

        ApplySlowdownClientRPC(playerID, clientRpcParams);
    }

    [ClientRpc]
    private void ApplySlowdownClientRPC(ulong playerID, ClientRpcParams clientRpcParams) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().OwnerClientId == playerID) {
                player.GetComponentInChildren<PlayerStats>().ApplySuperSlow();
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.transform.gameObject.tag == "PlayerTrigger" && other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            RemoveSlowdownServerRPC(other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveSlowdownServerRPC(ulong playerID) {
        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { playerID }
            }
        };

        RemoveSlowdownClientRPC(playerID, clientRpcParams);
    }

    [ClientRpc]
    private void RemoveSlowdownClientRPC(ulong playerID, ClientRpcParams clientRpcParams) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().OwnerClientId == playerID) {
                player.GetComponentInChildren<PlayerStats>().ClearSuperSlow();
            }
        }
    }
}
