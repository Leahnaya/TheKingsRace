using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using TMPro;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

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

    private bool isViewingRunnerControls = true;

    void Start()  {
        PauseMenuPanel.SetActive(false);
        ControlsPanel.SetActive(false);
        ConfirmationPanel.SetActive(false);
        RespawnConfirmationPanel.SetActive(false);
    }

    void Update() { 
        // Listen for Pause button and not already paused
        // TODO: UPDATE TO ALSO LISTEN FOR CONTROLLER PAUSE BUTTON PRESSED
        if (Input.GetKeyDown(KeyCode.Escape) && PauseMenuPanel.activeInHierarchy != true) {
            // Display Pause Menu
            PauseMenuPanel.SetActive(true);
            isViewingRunnerControls = true;

            // Unlock the cursor
            Cursor.lockState = CursorLockMode.None;

            // Disable player controls
            setPlayerControlsStateServerRPC(false);
        }
    }

    [ServerRpc]
    private void setPlayerControlsStateServerRPC(bool newState) {
        // Disable/Enable player controls
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            // Find the local player
            if (player.GetComponent<NetworkObject>().IsLocalPlayer) {
                // Have to use if checks since only the runners have these
                if (player.GetComponentInChildren<PlayerStats>() != null) {
                    player.GetComponentInChildren<PlayerStats>().IsPaused = !newState;
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

        // Lock the cursor
        Cursor.lockState = CursorLockMode.Locked;

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
        // Disable/Enable player controls
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) {
            // Find the local player
            if (player.GetComponent<NetworkObject>().IsLocalPlayer) {
                player.GetComponent<ResetZones>().RespawnToCheckpoint();
            }
        }
    }
}
