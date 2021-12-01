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
                        _king = Instantiate(kingPrefab, kingSpawnPoint, Quaternion.identity).gameObject;
                        _king.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, null, true);
                    } else {
                        // Spawn as player
                        _runner = Instantiate(runnerPrefab, runnersSpawnPoints[runnersSpawned], Quaternion.identity).gameObject;
                        _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(NetworkManager.Singleton.LocalClientId, null, true);

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
                    _king = Instantiate(kingPrefab, kingSpawnPoint, Quaternion.identity).gameObject;
                    _king.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);
                } else {
                    // Spawn as player
                    _runner = Instantiate(runnerPrefab, runnersSpawnPoints[runnersSpawned], Quaternion.identity).gameObject;
                    _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, null, true);

                    // Increment runners
                    runnersSpawned++;
                }
            }
        }
    }
}
