using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Goo : MonoBehaviour
{
    int Lifetime = 0;

    void FixedUpdate() {
        Lifetime++;
        if(Lifetime == 850) {
            if (NetworkManager.Singleton.IsServer) {
                DespawnMyselfServerRPC();
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.gameObject.tag == "PlayerTrigger" && other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            ApplyGooServerRPC(other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyGooServerRPC(ulong playerID) {
        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { playerID }
            }
        };

        ApplyGooClientRPC(playerID, clientRpcParams);
    }

    [ClientRpc]
    private void ApplyGooClientRPC(ulong playerID, ClientRpcParams clientRpcParams) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().OwnerClientId == playerID) {
                player.GetComponentInChildren<PlayerStats>().ApplySlimeTrail();
            }
        }
    }

    void OnTriggerExit(Collider other) {
        if (other.transform.gameObject.tag == "PlayerTrigger" && other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            RemoveGooServerRPC(other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void RemoveGooServerRPC(ulong playerID) {
        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { playerID }
            }
        };

        RemoveGooClientRPC(playerID, clientRpcParams);
    }

    [ClientRpc]
    private void RemoveGooClientRPC(ulong playerID, ClientRpcParams clientRpcParams) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            if (player.GetComponent<NetworkObject>().OwnerClientId == playerID) {
                player.GetComponentInChildren<PlayerStats>().ClearSlimeTrail();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnMyselfServerRPC() {
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
