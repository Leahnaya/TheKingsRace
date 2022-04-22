using System;
using System.Collections;
using MLAPI;
using MLAPI.Connection;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using MLAPI.NetworkVariable.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class LobbyUI : NetworkBehaviour {

    [Header("References")]
    [SerializeField] private LobbyPlayerCard[] lobbyPlayerCards;
    [SerializeField] private Button startGameButton;
    [SerializeField] private TMP_Text lobbyStateText;
    [SerializeField] private TMP_Text hostIpAddressText;

    private NetworkList<LobbyPlayerState> lobbyPlayers = new NetworkList<LobbyPlayerState>();
    private NetworkVariable<String> hostIpAddress = new NetworkVariable<String>();

    private string LobbyStatusText = "";

    public override void NetworkStart()
    {
        // Unlock the player cursor if they get sent back here after a match
        Cursor.lockState = CursorLockMode.None;

        if (IsClient)
        {
            lobbyPlayers.OnListChanged += HandleLobbyPlayersStateChanged;
        }

        if (IsServer)
        {
            startGameButton.gameObject.SetActive(true);

            NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnect;

            foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
            {
                HandleClientConnected(client.ClientId);
            }

            // Get/Set the ip of the host
            StartCoroutine(GetIPAddress());

            UpdateLobbyStateText();
        }

        // Reset the variables for IsReady and IsKing
        if (IsHost) {
            for (int i = 0; i < lobbyPlayers.Count; i++) {
                // Update the lobby player cards states
                lobbyPlayers[i] = new LobbyPlayerState(
                    lobbyPlayers[i].ClientId,
                    lobbyPlayers[i].PlayerName,
                    false,
                    false
                );

                // Update the server's player data as well
                if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(lobbyPlayers[i].ClientId, out string clientGuid)) {
                    ServerGameNetPortal.Instance.clientData[clientGuid] = new PlayerData(
                        ServerGameNetPortal.Instance.clientData[clientGuid].PlayerName,
                        ServerGameNetPortal.Instance.clientData[clientGuid].ClientId,
                        false
                    );
                }
            }
        }
    }

    void Start() {
        ToggleReadyServerRpc(); 
        ToggleReadyServerRpc();
    }

    private void OnDestroy()
    {
        lobbyPlayers.OnListChanged -= HandleLobbyPlayersStateChanged;

        if (NetworkManager.Singleton)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= HandleClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback -= HandleClientDisconnect;
        }
    }

    private bool IsEveryoneReady()
    {
        // Make sure there is 3 players in the lobby
        if (lobbyPlayers.Count != 3)
        {
            lobbyStateText.text = "Need 3 Players To Begin!";
            return false;
        }

        foreach (var player in lobbyPlayers)
        {
            if (!player.IsReady)
            {
                lobbyStateText.text = "All Players Must Ready Up To Begin!";
                return false;
            }
        }

        return true;
    }

    private bool AreRolesFilled() {
        int runnerCount = 0;
        int kingCount = 0;
        foreach (var player in lobbyPlayers) {
            // Check how many of each player
            if (player.IsKing) {
                kingCount++;
            } else {
                runnerCount++;
            }
        }

        // Verify counts
        if (runnerCount != 2 || kingCount != 1) {
            return false;
        }

        return true;
    }

    private void HandleClientConnected(ulong clientId)
    {
        var playerData = ServerGameNetPortal.Instance.GetPlayerData(clientId);

        if (!playerData.HasValue) { return; }

        lobbyPlayers.Add(new LobbyPlayerState(
            clientId,
            playerData.Value.PlayerName,
            false,
            false
        ));
    }

    private void HandleClientDisconnect(ulong clientId)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == clientId)
            {
                lobbyPlayers.RemoveAt(i);
                break;
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ToggleReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        for (int i = 0; i < lobbyPlayers.Count; i++)
        {
            if (lobbyPlayers[i].ClientId == serverRpcParams.Receive.SenderClientId)
            {
                lobbyPlayers[i] = new LobbyPlayerState(
                    lobbyPlayers[i].ClientId,
                    lobbyPlayers[i].PlayerName,
                    !lobbyPlayers[i].IsReady,
                    lobbyPlayers[i].IsKing
                );
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwapTeamsServerRpc(ServerRpcParams serverRpcParams = default) { 
        for (int i = 0; i < lobbyPlayers.Count; i++) {
            ulong senderClientID = serverRpcParams.Receive.SenderClientId;
            // Update the lobby state that controls that player's data
            if (lobbyPlayers[i].ClientId == senderClientID) {
                lobbyPlayers[i] = new LobbyPlayerState(
                    lobbyPlayers[i].ClientId,
                    lobbyPlayers[i].PlayerName,
                    lobbyPlayers[i].IsReady,
                    !lobbyPlayers[i].IsKing
                );

                // Update the server's player data as well
                if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(senderClientID, out string clientGuid)) {
                    ServerGameNetPortal.Instance.clientData[clientGuid] = new PlayerData(
                        ServerGameNetPortal.Instance.clientData[clientGuid].PlayerName,
                        ServerGameNetPortal.Instance.clientData[clientGuid].ClientId,
                        lobbyPlayers[i].IsKing
                    );
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdatePlayerInventoryServerRpc(string itemName, int add, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientID = serverRpcParams.Receive.SenderClientId;
        if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(senderClientID, out string clientGuid)) {
                    ServerGameNetPortal.Instance.clientData[clientGuid].pInv.UpdateItemNetwork(itemName, add);
                    print("Item in Network updated");
        }
    }

    public void EquipItems(Item item, bool ableToAdd){
        //Update the Local player Inventory
        int added = FindObjectOfType<PlayerInventory>().UpdateInventory(item, ableToAdd);
        FindObjectOfType<PlayerInventory>().UpdateItemNetwork(item.name, added);

        //Server RPC Update player Inventory
        UpdatePlayerInventoryServerRpc(item.name, added);
    }


    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        if (serverRpcParams.Receive.SenderClientId != NetworkManager.Singleton.LocalClientId) { return; }

        if (!IsEveryoneReady() && !AreRolesFilled()) { return; }

        ServerGameNetPortal.Instance.StartGame();
    }

    public void OnLeaveClicked()
    {
        GameNetPortal.Instance.RequestDisconnect();
    }

    public void OnReadyClicked()
    {
        ToggleReadyServerRpc();
    }

    public void OnStartGameClicked()
    {
        StartGameServerRpc();
    }

    public void OnSwapTeamClicked() {
        SwapTeamsServerRpc();
    }

    private void HandleLobbyPlayersStateChanged(NetworkListEvent<LobbyPlayerState> lobbyState) { 
        for (int i = 0; i < lobbyPlayerCards.Length; i++)
        {
            if (lobbyPlayers.Count > i) {
                lobbyPlayerCards[i].UpdateDisplay(lobbyPlayers[i]);
                //Debug.Log("A");
            }
            else {
                lobbyPlayerCards[i].DisableDisplay();
                //Debug.Log("B");
            }
        }

        if(IsHost)
        {
            if (IsEveryoneReady() && AreRolesFilled()) {
                startGameButton.interactable = true;
            } else {
                startGameButton.interactable = false;
            }
        }

        if (IsServer) { 
            UpdateLobbyStateText();
        }
    }

    private void UpdateLobbyStateText() {

        if (lobbyPlayers.Count != 3) {
            LobbyStatusText = "Need 3 Players to Begin!";
            if (IsServer) {
                UpdateLobbyStateTextServerRPC(LobbyStatusText);
            }
            return;
        }

        int runnerCount = 0;
        int kingCount = 0;
        foreach (var player in lobbyPlayers) {
            // Check how many of each player
            if (player.IsKing) {
                kingCount++;
            } else {
                runnerCount++;
            }
        }

        if (IsEveryoneReady()) {

            // Verify counts
            if (runnerCount != 2 || kingCount != 1) {
                LobbyStatusText = "Need 2 Runners and 1 King!";
                if (IsServer) {
                    UpdateLobbyStateTextServerRPC(LobbyStatusText);
                }
                return;
            }

            LobbyStatusText = "All Players Ready!";
        }
        else {
            LobbyStatusText = "All Players Need to Ready Up!";
        }
        
        if (IsServer) {
            UpdateLobbyStateTextServerRPC(LobbyStatusText);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateLobbyStateTextServerRPC(string stateText) {
        LobbyStatusText = stateText;
        UpdateLobbyStateTextClientRPC(stateText);
    }

    [ClientRpc]
    private void UpdateLobbyStateTextClientRPC(string stateText) {
        LobbyStatusText = stateText;
    }

    // Code by: Goodgulf
    IEnumerator GetIPAddress() {
        UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            hostIpAddress.Value = "Network Error - Try Again";
        } else {
            string result = www.downloadHandler.text;

            // This results in a string similar to this: <html><head><title>Current IP Check</title></head><body>Current IP Address: 123.123.123.123</body></html>
            // where 123.123.123.123 is your external IP Address.
            //  Debug.Log("" + result);

            string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
            string a2 = a[1].Substring(1);  // Get the substring after the :
            string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
            string a4 = a3[0];              // Get the substring before the tag.

            hostIpAddress.Value = "Host IP: " + a4;
        }
    }

    public void RefreshIPAddress() {
        if (!IsServer) { return; }
        StartCoroutine(GetIPAddress());
    }

    void Update() {
        hostIpAddressText.text = hostIpAddress.Value;
        lobbyStateText.text = LobbyStatusText;
    }
}
