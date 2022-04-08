using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MLAPI;
using MLAPI.Exceptions;
using MLAPI.Messaging;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : NetworkBehaviour {

    [SerializeField]
    private GameObject PauseMenuPanel;

    [SerializeField]
    private GameObject ControlsPanel;

    [SerializeField]
    private GameObject ConfirmationPanel;

    [SerializeField]
    private GameObject RespawnConfirmationPanel;

    [SerializeField]
    private TMP_Text ControlsHeader;
    [SerializeField]
    private TMP_Text ControlsButtonText;
    [SerializeField]
    private TMP_Text ControlsText;

    [SerializeField]
    private Button RestartButton;

    public Transform runnerPrefab;

    private GameObject _runner;

    private bool isViewingRunnerControls = true;

    private ResetZonesGlobal[] respawnZones;

    public bool isUsable = false;

    void Awake() {
        respawnZones = (ResetZonesGlobal[])GameObject.FindObjectsOfType(typeof(ResetZonesGlobal));
    }

    void Start()  {
        PauseMenuPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        ConfirmationPanel.SetActive(false);
        RespawnConfirmationPanel.SetActive(false);
    }

    void Update() { 
        // Listen for Pause button and not already paused
        // TODO: UPDATE TO ALSO LISTEN FOR CONTROLLER PAUSE BUTTON PRESSED
        if (isUsable && Input.GetKeyDown(KeyCode.Escape) && PauseMenuPanel.activeInHierarchy != true) {
            // Display Pause Menu
            PauseMenuPanel.SetActive(true);
            isViewingRunnerControls = true;

            // Disable player controls
            setPlayerControlsStateServerRPC(false);

            // Turn off "Restart from Checkpoint" button for the king
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject player in players) { 
                // Make sure they are the local player
                if (player.GetComponent<NetworkObject>().IsLocalPlayer) {
                    // Check for king
                    if (player.GetComponent<KingMove>() != null) {
                        RestartButton.interactable = false;
                    }
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void setPlayerControlsStateServerRPC(bool newState, ServerRpcParams serverRpcParams = default) {
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
            }
        };

        setPlayerControlsStateClientRPC(newState, clientRpcParams);
    }

    [ClientRpc]
    private void setPlayerControlsStateClientRPC(bool newState, ClientRpcParams clientRpcParams) {
        // Disable/Enable player controls
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            // Find the local player
            if (player.GetComponent<NetworkObject>().IsLocalPlayer) {
                // Have to use if checks since only the runners have these
                if (player.GetComponentInChildren<PlayerStats>() != null) {
                    player.GetComponentInChildren<PlayerStats>().IsPaused = !newState;

                    // Lock the cursor
                    if (newState) {
                        Cursor.lockState = CursorLockMode.Locked;
                        Debug.Log("Locked");
                    } else {
                        Cursor.lockState = CursorLockMode.None;
                        Debug.Log("Unlocked");
                    }
                }

                // King's Movement
                if (player.GetComponent<KingMove>() != null) {
                    player.GetComponent<KingMove>().enabled = newState;
                }
            }
        }
    }

    public void OnResumeGameClicked() {
        // Unlock player controls
        setPlayerControlsStateServerRPC(true);

        // Hide PauseMenu
        PauseMenuPanel.SetActive(false);
    }

    public void OnControlsClicked() {
        ControlsPanel.SetActive(true);
    }

    public void OnQuitGameClicked() {
        ConfirmationPanel.SetActive(true);
    }

    public void OnConfirmationYesClicked() {
        // Leave game
        GameNetPortal.Instance.RequestDisconnect();
    }

    public void OnConfirmationNoClicked() {
        ConfirmationPanel.SetActive(false);
    }

    public void OnBackButtonClicked() {
        ControlsPanel.SetActive(false);
        isViewingRunnerControls = true;
    }

    // Used to swap displaying runner/king controls
    public void ToggleControlsScreen() { 
        if (isViewingRunnerControls) {
            isViewingRunnerControls = false;
            ControlsHeader.text = "King Controls";
            ControlsButtonText.text = "Runner Controls";
            ControlsText.text = "To-Do: Put the King's Controls here";
        } else {
            isViewingRunnerControls = true;
            ControlsHeader.text = "Runner Controls";
            ControlsButtonText.text = "King Controls";
            ControlsText.text = "To-Do: Put the Runner's Controls here";
        }
    }

    public void OnRespawnClicked() {
        RespawnConfirmationPanel.SetActive(true);
    }

    public void RespawnDecline() {
        RespawnConfirmationPanel.SetActive(false);
    }

    public void RespawnConfirmed() {
        // Turn off the pause menu
        RespawnDecline();
        OnResumeGameClicked();

        // Respawn the player
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            // Find the local player
            if (player.GetComponent<NetworkObject>().IsLocalPlayer) {
                ResetZones.Zone curZone = ResetZones.Zone.VALLEY;
                foreach(ResetZonesGlobal reZone in respawnZones) {
                    if (reZone.gameObject.GetComponent<BoxCollider>().bounds.Intersects(player.GetComponentInChildren<CapsuleCollider>().bounds)) {
                        curZone = (ResetZones.Zone)reZone.RespawnZone;
                    }
                }

                RespawnPlayerServerRPC(player.GetComponent<NetworkObject>().OwnerClientId, 
                    player.GetComponentInChildren<ResetZones>().GetRespawnPosition(curZone));
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RespawnPlayerServerRPC(ulong clientID, Vector3 respawnPos, ServerRpcParams serverRpcParams = default) {

        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Find our player first
        foreach (GameObject character in playableCharacters) {
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientID) {
                // Player found

                // Despawn them
                try {
                    character.GetComponent<NetworkObject>().Despawn(true);
                } catch (SpawnStateException e) {
                    Debug.LogError("Spawn State Exception Exception:");
                    Debug.LogError(e);
                    return;
                }
                //DespawnPlayerServerRPC(clientID);

                // Spawn the player
                if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(clientID, out string clientGuid)) {
                    if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData)) {
                        // Spawn as player
                        _runner = Instantiate(runnerPrefab, respawnPos, Quaternion.Euler(0, -90, 0)).gameObject;
                        _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, null, true);

                        // Handle Client Spawning Locally
                        string itemsAsString = string.Join(",", playerData.pInv.NetworkItemList);
                        
                        StartCoroutine(SpawnClient(clientID, itemsAsString));
                    }
                }
            }
        }
    }

    // The client respawning needs a slight delay to allow for the spawn to properly sync up
    IEnumerator SpawnClient(ulong clientID, string itemsAsString) {
        // First notify the player they are respawning (so we display stuff on screen)
        ClientRpcParams clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        UIRespawningClientRpc(clientID, clientRpcParams);

        // 3 Second Respawn Delay
        yield return new WaitForSecondsRealtime(3f);
        // Now actually respawn the player
        clientRpcParams = new ClientRpcParams {
            Send = new ClientRpcSendParams {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        SpawnPlayerClientRpc(clientID, itemsAsString, clientRpcParams);
    }

    [ClientRpc]
    private void UIRespawningClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    {
        GameObject.FindGameObjectWithTag("RunnerHUD").GetComponent<PlayerHUD>().setRespawnPanelVisibility(true);
        GameObject.FindGameObjectWithTag("RunnerHUD").GetComponent<PlayerHUD>().countdown_text.enabled = false;
    }

    // Spawn in each player
    [ClientRpc]
    public void SpawnPlayerClientRpc(ulong clientId, string itemsAsString, ClientRpcParams clientRpcParams = default) {
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playableCharacters) {
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientId) {
                // Rebuild inventory
                List<string> itemList = itemsAsString.Split(',').ToList();
                character.GetComponentInChildren<PlayerInventory>().UpdateEquips(itemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);

                GameObject UICamera = GameHandler.FindGameObjectInChildWithTag(character, "UICam");

                GameHandler.FindGameObjectInChildWithTag(character, "UICam").GetComponent<Camera>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(UICamera, "PlayerCam").GetComponent<Camera>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(UICamera, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(UICamera, "PlayerCam").GetComponent<PlayerCam>().enabled = true;

                character.GetComponentInChildren<MoveStateManager>().enabled = true;
                character.GetComponentInChildren<DashStateManager>().enabled = true;
                character.GetComponentInChildren<NitroStateManager>().enabled = true;
                character.GetComponentInChildren<AerialStateManager>().enabled = true;
                character.GetComponentInChildren<OffenseStateManager>().enabled = true;
                character.GetComponentInChildren<CoolDown>().populatePlayerCanvas();
            }
        }
        
        // Also turn off the respawning UI and back on the UI for the timer
        GameObject.FindGameObjectWithTag("RunnerHUD").GetComponent<PlayerHUD>().setRespawnPanelVisibility(false);
        GameObject.FindGameObjectWithTag("RunnerHUD").GetComponent<PlayerHUD>().countdown_text.enabled = true;
    }
}
