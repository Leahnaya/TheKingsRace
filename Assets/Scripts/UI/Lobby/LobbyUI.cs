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
        }
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
    private void UpdatePlayerInventoryServerRpc(String item, ServerRpcParams serverRpcParams = default)
    {
        ulong senderClientID = serverRpcParams.Receive.SenderClientId;
    //     if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(senderClientID, out string clientGuid)) {
    //                 ServerGameNetPortal.Instance.clientData[clientGuid].pInv.AddItemNetwork(item);
    // }
    }

    // public void EquipItems(String item, bool ableToAdd){
    //     //Update the Local player Inventory
    //     FindObjectOfType<PlayerInventory>().UpdateInventory(item, ableToAdd);
        
    //     //Call the server RPC
    //     UpdatePlayerInventoryServerRpc(item);
    // }

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
            if (lobbyPlayers.Count > i)
            {
                lobbyPlayerCards[i].UpdateDisplay(lobbyPlayers[i]);
            }
            else
            {
                lobbyPlayerCards[i].DisableDisplay();
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

        UpdateLobbyStateText();
    }

    private void UpdateLobbyStateText() {

        if (lobbyPlayers.Count != 3) {
            lobbyStateText.text = "Need 3 Players to Begin!";
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
                lobbyStateText.text = "Need 2 Runners and 1 King!";
                return;
            }

            lobbyStateText.text = "All Players Ready!";
        }
        else {
            lobbyStateText.text = "All Players Need to Ready Up!";
        }
    }

    //Taken from Goodgulf
    IEnumerator GetIPAddress() {
        UnityWebRequest www = UnityWebRequest.Get("http://checkip.dyndns.org");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError) {
            hostIpAddress.Value = "Host IP: Network Error";
        } else {
            string result = www.downloadHandler.text;

            // This results in a string similar to this: <html><head><title>Current IP Check</title></head><body>Current IP Address: 123.123.123.123</body></html>
            // where 123.123.123.123 is your external IP Address.
            //  Debug.Log("" + result);

            string[] a = result.Split(':'); // Split into two substrings -> one before : and one after. 
            string a2 = a[1].Substring(1);  // Get the substring after the :
            string[] a3 = a2.Split('<');    // Now split to the first HTML tag after the IP address.
            string a4 = a3[0];              // Get the substring before the tag.

            Debug.Log("External IP Address = " + a4);
            hostIpAddress.Value = "Host IP: " + a4;
        }
    }

    void Update() {
        hostIpAddressText.text = hostIpAddress.Value;
    }
}
