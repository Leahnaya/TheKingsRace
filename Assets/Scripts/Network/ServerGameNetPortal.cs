using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using MLAPI;
using MLAPI.SceneManagement;
using MLAPI.Spawning;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ServerGameNetPortal : MonoBehaviour {

    [Header("Settings")]
    [SerializeField] private int maxPlayers = 3;

    [Header("Prefabs")]
    [SerializeField] private GameObject runnerPrefab;
    [SerializeField] private GameObject kingPrefab;

    public static ServerGameNetPortal Instance => instance;
    private static ServerGameNetPortal instance;

    public Dictionary<string, PlayerData> clientData;
    public Dictionary<ulong, string> clientIdToGuid;
    private Dictionary<ulong, int> clientSceneMap;
    private bool gameInProgress;

    private const int MaxConnectionPayload = 1024;

    private GameNetPortal gameNetPortal;

    private int gameLevelLoaded = -1;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        gameNetPortal = GetComponent<GameNetPortal>();
        gameNetPortal.OnNetworkReadied += HandleNetworkReadied;

        NetworkManager.Singleton.ConnectionApprovalCallback += ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted += HandleServerStarted;

        clientData = new Dictionary<string, PlayerData>();
        clientIdToGuid = new Dictionary<ulong, string>();
        clientSceneMap = new Dictionary<ulong, int>();
    }

    private void OnDestroy()
    {
        if (gameNetPortal == null) { return; }

        gameNetPortal.OnNetworkReadied -= HandleNetworkReadied;

        if (NetworkManager.Singleton == null) { return; }

        NetworkManager.Singleton.ConnectionApprovalCallback -= ApprovalCheck;
        NetworkManager.Singleton.OnServerStarted -= HandleServerStarted;
    }

    public PlayerData? GetPlayerData(ulong clientId)
    {
        if (clientIdToGuid.TryGetValue(clientId, out string clientGuid))
        {
            if (clientData.TryGetValue(clientGuid, out PlayerData playerData))
            {
                return playerData;
            }
            else
            {
                Debug.LogWarning($"No player data found for client id: {clientId}");
            }
        }
        else
        {
            Debug.LogWarning($"No client guid found for client id: {clientId}");
        }

        return null;
    }

    public void StartGame()
    {
        gameInProgress = true;

        // Load the mountain level.  Can add code to swap which level here
        gameLevelLoaded = 0;
        NetworkSceneManager.SwitchScene("Game-Mountain");

        // Start the round
        BeginRound();
    }

    // Code to handle the beginning of round logic where we do the camera movement and spawning in players
    public void BeginRound() {
        Vector3[] runnersSpawnPoints;
        Vector3 kingSpawnPoint;

        // Get the spawn points for the level
        switch (gameLevelLoaded) {
            default:
            case 0:
                runnersSpawnPoints = SpawnPoints.Instance.getRunnerSpawnPoints(gameLevelLoaded);
                kingSpawnPoint = SpawnPoints.Instance.getKingSpawnPoint(gameLevelLoaded);
                break;
        }

        // Spawn in the players, but make sure the character controllers are diabled
        int runnerSpawnIndex = 0;
        foreach (PlayerData pData in clientData.Values) {
            // Spawn in the prefab for the player based on king or runner
            GameObject go = null;
            if (pData.IsKing) {
                go = Instantiate(runnerPrefab, kingSpawnPoint, Quaternion.identity);
                
            } else {
                go = Instantiate(runnerPrefab, runnersSpawnPoints[runnerSpawnIndex], Quaternion.identity);
            }
            
            // Disable the character controller
            go.GetComponent<CharacterController>().enabled = false;
            go.GetComponent<CapsuleCollider>().enabled = true;

            // Spawn the player on network and assign the owner
            go.GetComponent<NetworkObject>().Spawn();
            go.GetComponent<NetworkObject>().ChangeOwnership(pData.ClientId);
        }

        // Perform the intro cutscene camera movement

        // Do a 3.2.1 countdown or ask team what we want to do here

        // Re-enable the character controllers and disable the capsulecollider

        // Start the game timer
    }

    public void EndRound()
    {
        gameInProgress = false;

        NetworkSceneManager.SwitchScene("Lobby");
    }

    private void HandleNetworkReadied()
    {
        if (!NetworkManager.Singleton.IsServer) { return; }

        gameNetPortal.OnUserDisconnectRequested += HandleUserDisconnectRequested;
        NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;
        gameNetPortal.OnClientSceneChanged += HandleClientSceneChanged;

        NetworkSceneManager.SwitchScene("Lobby");

        if (NetworkManager.Singleton.IsHost)
        {
            clientSceneMap[NetworkManager.Singleton.LocalClientId] = SceneManager.GetActiveScene().buildIndex;
        }
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        clientSceneMap.Remove(clientId);

        if (clientIdToGuid.TryGetValue(clientId, out string guid))
        {
            clientIdToGuid.Remove(clientId);

            if (clientData[guid].ClientId == clientId)
            {
                clientData.Remove(guid);
            }
        }

        if (clientId == NetworkManager.Singleton.LocalClientId)
        {
            gameNetPortal.OnUserDisconnectRequested -= HandleUserDisconnectRequested;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
            gameNetPortal.OnClientSceneChanged -= HandleClientSceneChanged;
        }
    }

    private void HandleClientSceneChanged(ulong clientId, int sceneIndex)
    {
        clientSceneMap[clientId] = sceneIndex;
    }

    private void HandleUserDisconnectRequested()
    {
        HandleClientDisconnect(NetworkManager.Singleton.LocalClientId);

        NetworkManager.Singleton.StopHost();

        ClearData();

        SceneManager.LoadScene("TitleScene");
    }

    private void HandleServerStarted()
    {
        if (!NetworkManager.Singleton.IsHost) { return; }

        string clientGuid = Guid.NewGuid().ToString();
        string playerName = PlayerPrefs.GetString("PlayerName", "Missing Name");

        clientData.Add(clientGuid, new PlayerData(playerName, NetworkManager.Singleton.LocalClientId));
        clientIdToGuid.Add(NetworkManager.Singleton.LocalClientId, clientGuid);
    }

    private void ClearData()
    {
        clientData.Clear();
        clientIdToGuid.Clear();
        clientSceneMap.Clear();

        gameInProgress = false;
    }

    private void ApprovalCheck(byte[] connectionData, ulong clientId, NetworkManager.ConnectionApprovedDelegate callback)
    {
        if (connectionData.Length > MaxConnectionPayload)
        {
            callback(false, 0, false, null, null);
            return;
        }

        string payload = Encoding.UTF8.GetString(connectionData);
        var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload);

        ConnectStatus gameReturnStatus = ConnectStatus.Success;

        // This stops us from running multiple standalone builds since 
        // they disconnect eachother when trying to join
        //
        // if (clientData.ContainsKey(connectionPayload.clientGUID))
        // {
        //     ulong oldClientId = clientData[connectionPayload.clientGUID].ClientId;
        //     StartCoroutine(WaitToDisconnectClient(oldClientId, ConnectStatus.LoggedInAgain));
        // }

        if (gameInProgress)
        {
            gameReturnStatus = ConnectStatus.GameInProgress;
        }
        else if (clientData.Count >= maxPlayers)
        {
            gameReturnStatus = ConnectStatus.ServerFull;
        }

        if (gameReturnStatus == ConnectStatus.Success)
        {
            clientSceneMap[clientId] = connectionPayload.clientScene;
            clientIdToGuid[clientId] = connectionPayload.clientGUID;
            clientData[connectionPayload.clientGUID] = new PlayerData(connectionPayload.playerName, clientId);
        }

        callback(false, 0, true, null, null);

        gameNetPortal.ServerToClientConnectResult(clientId, gameReturnStatus);

        if (gameReturnStatus != ConnectStatus.Success)
        {
            StartCoroutine(WaitToDisconnectClient(clientId, gameReturnStatus));
        }
    }

    private IEnumerator WaitToDisconnectClient(ulong clientId, ConnectStatus reason)
    {
        gameNetPortal.ServerToClientSetDisconnectReason(clientId, reason);

        yield return new WaitForSeconds(0);

        KickClient(clientId);
    }

    private void KickClient(ulong clientId)
    {
        NetworkObject networkObject = NetworkSpawnManager.GetPlayerNetworkObject(clientId);
        if (networkObject != null)
        {
            networkObject.Despawn(true);
        }

        NetworkManager.Singleton.DisconnectClient(clientId);
    }
}
