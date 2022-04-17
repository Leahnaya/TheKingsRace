using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnManager : NetworkBehaviour {

    public Transform runnerPrefab;
    public Transform kingPrefab;

    private GameObject _runner;
    private GameObject _king;

    private Vector3[] runnersSpawnPoints;
    private Vector3 kingSpawnPoint;

    private static int runnersSpawned = 0;

    // Spawn in the players on load
    void Start() {
        runnersSpawned = 0;

        InitSpawnPoints();

        if (IsHost) {
            // Get the player data for the host player
            if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(NetworkManager.Singleton.LocalClientId, out string clientGuid)) {
                if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData)) {
                    if (playerData.IsKing) {
                        // Spawn as king
                        _king = Instantiate(kingPrefab, kingSpawnPoint, Quaternion.Euler(0, 180, 0)).gameObject;
                        _king.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, null, true);
                    } else {
                        // Spawn as player
                        _runner = Instantiate(runnerPrefab, runnersSpawnPoints[runnersSpawned], Quaternion.Euler(0, -90, 0)).gameObject;
                        //Recreate Inventory
                        _runner.GetComponentInChildren<PlayerInventory>().UpdateEquips(playerData.pInv.NetworkItemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);
                        _runner.GetComponentInChildren<CoolDown>().populatePlayerCanvas();
                        _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, null, true);
                        _runner.GetComponentInChildren<PlayerStats>().IsRespawning = false;
                        // Increment runners
                        runnersSpawned++;
                    }
                }
            }
        } else {
            // Spawn via RPC on the server
            SpawnPlayerServerRpc(NetworkManager.Singleton.LocalClientId);
        }
    }

    private void InitSpawnPoints() { 
        // Get the spawn points for the level
        if (SceneManager.GetActiveScene().buildIndex == 3) { // Mountain Level
            runnersSpawnPoints = SpawnPoints.Instance.getRunnerSpawnPoints(0);
            kingSpawnPoint = SpawnPoints.Instance.getKingSpawnPoint(0);
            
        }
    }

    // Spawn in each player
    [ServerRpc(RequireOwnership = false)]
    public void SpawnPlayerServerRpc(ulong clientId) {
        // Get the player data for the player calling the spawn
        if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(clientId, out string clientGuid)) {
            if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData)) {
                if (playerData.IsKing) {
                    // Spawn as king
                    _king = Instantiate(kingPrefab, kingSpawnPoint, Quaternion.Euler(0, 180, 0)).gameObject;
                    _king.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
                } else {
                    // Spawn as player
                    _runner = Instantiate(runnerPrefab, runnersSpawnPoints[runnersSpawned], Quaternion.Euler(0, -90, 0)).gameObject;
                    _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
                    _runner.GetComponentInChildren<PlayerStats>().IsRespawning = false;
                    // Increment runners
                    runnersSpawned++;

                    // Tell the client to apply the inventory to its player
                    string itemsAsString = string.Join(",", playerData.pInv.NetworkItemList);
                    StartCoroutine(ApplyInventoryToClient(clientId, itemsAsString));
                }
            }
        }
    }

    // Give a slight delay to allow spawning to complete
    IEnumerator ApplyInventoryToClient(ulong clientID, string itemsAsString)
    {
        yield return new WaitForSecondsRealtime(5f);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        ApplyInventoryClientRPC(clientID, itemsAsString, clientRpcParams);
    }

    [ClientRpc]
    private void ApplyInventoryClientRPC(ulong clientID, string itemsAsString, ClientRpcParams clientRpcParams = default) {
        

        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Loop over the characters
        foreach (GameObject character in playableCharacters) {
            // Find the local player
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientID) {
                List<string> itemList = itemsAsString.Split(',').ToList();
                character.GetComponentInChildren<PlayerInventory>().UpdateEquips(itemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);
                character.GetComponentInChildren<CoolDown>().populatePlayerCanvas();
                character.GetComponentInChildren<PlayerStats>().IsRespawning = false;
                
            }
        }
    }
}
