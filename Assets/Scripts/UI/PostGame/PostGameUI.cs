using MLAPI;
using MLAPI.Messaging;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameUI : NetworkBehaviour {

    [SerializeField] private TMP_Text ReturnToLobbyText;
    [SerializeField] private int ReturnToLobbyTimer;

    [SerializeField] private TMP_Text HeaderText;
    [SerializeField] private GameObject king;
    [SerializeField] private GameObject player1;
    [SerializeField] private GameObject player2;

    public AudioSource kingWin;
    public AudioSource runnerWin;

    void Start() {
        ReturnToLobbyText.text = "Returning to the lobby in " + ReturnToLobbyTimer + " seconds...";
        StartCoroutine(BeginCountdown());

        UpdateWinnerTextServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateWinnerTextServerRPC() {
        List<string> playerFinishedNames = new List<string>();
        string kingName = "";
        int winnerCount = 0;

        string winText = "";

        foreach (PlayerData pData in ServerGameNetPortal.Instance.clientData.Values) {
            if (pData.IsKing) {
                kingName = pData.PlayerName;
            } else {
                if (pData.Finished)
                {
                    playerFinishedNames.Add(pData.PlayerName);
                    winnerCount++;
                }
            }
        }
        bool[] active = { false, false, false };
        if (winnerCount > 0) {
            // Runners win
            switch (winnerCount) {
                case 1:
                    runnerWin.Play();
                    winText = "And the winner is the Runner " + playerFinishedNames[0] + "!";
                    active[1] = true;
                    player1.SetActive(active[1]);
                    break;
                case 2:
                    runnerWin.Play();
                    winText = "And the winners are the Runners " + playerFinishedNames[0] + " and " + playerFinishedNames[1] + "!";
                    active[1] = true;
                    active[2] = true;
                    player1.SetActive(active[1]);
                    player2.SetActive(active[2]);
                    break;
            }
        } else {
            // King wins
            kingWin.Play();
            winText = "And the winner is King " + kingName + "!";
            active[0] = true;
            king.SetActive(active[0]);
        }

        UpdateWinnerTextClientRPC(winText, active);
    }

    [ClientRpc]
    private void UpdateWinnerTextClientRPC(string winnerText, bool[] active) {
        HeaderText.text = winnerText;
        king.SetActive(active[0]);
        player1.SetActive(active[1]);
        player2.SetActive(active[2]);
    }

    IEnumerator BeginCountdown() { 
        for (int i = ReturnToLobbyTimer; i >= 0; i--) {
            ReturnToLobbyText.text = "Returning to the lobby in " + i + " seconds...";
            yield return new WaitForSecondsRealtime(1f);
        }

        // Have the host return everyone to the lobby
        if (IsHost) {
            ReturnToLobbyServerRPC();
        }
    }

    [ServerRpc]
    private void ReturnToLobbyServerRPC() {
        ServerGameNetPortal.Instance.EndRound();
    }
}
