using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pitfall : NetworkBehaviour
{
    [SerializeField] private Vector3 RespawnPoint; //Where player will respawn to, set in GUI

    public Transform runnerPrefab;

    private GameObject _runner;

    private void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject.tag == "Player" && other.gameObject.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            RespawnPlayerServerRPC();
        }
    }

    [ServerRpc]
    private void RespawnPlayerServerRPC(ServerRpcParams serverRpcParams = default)
    {
        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Find our player first
        foreach (GameObject character in playableCharacters)
        {
            if (character.GetComponent<NetworkObject>().OwnerClientId == serverRpcParams.Receive.SenderClientId)
            {
                // Player found

                // Despawn them
                character.GetComponent<NetworkObject>().Despawn(true);
            }
        }

        if (IsHost)
        {
            // Get the player data for the host player
            if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(serverRpcParams.Receive.SenderClientId, out string clientGuid))
            {
                if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData))
                {
                    // Spawn as player
                    _runner = Instantiate(runnerPrefab, RespawnPoint, Quaternion.Euler(0, -90, 0)).gameObject;
                    //Recreate Inventory
                    _runner.GetComponentInChildren<PlayerInventory>().UpdateEquips(playerData.pInv.NetworkItemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);
                    _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(serverRpcParams.Receive.SenderClientId, null, true);

                    GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<Camera>().enabled = true;
                    GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                    GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<PlayerCam>().enabled = true;

                    _runner.GetComponentInChildren<PlayerMovement>().enabled = true;
                }
            }
        }
        else
        {
            // Spawn via RPC on the server
            SpawnPlayerServerRpc(serverRpcParams.Receive.SenderClientId);
        }
    }

    // Spawn in each player
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId)
    {
        // Get the player data for the player calling the spawn
        if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(clientId, out string clientGuid))
        {
            if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData))
            {
                // Spawn as player
                _runner = Instantiate(runnerPrefab, RespawnPoint, Quaternion.Euler(0, -90, 0)).gameObject;
                //Recreate Inventory
                _runner.GetComponentInChildren<PlayerInventory>().UpdateEquips(playerData.pInv.NetworkItemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);
                _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);

                GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<Camera>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<PlayerCam>().enabled = true;

                _runner.GetComponentInChildren<PlayerMovement>().enabled = true;
            }
        }
    }
}
