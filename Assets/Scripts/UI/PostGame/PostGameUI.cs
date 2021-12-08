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

    void Start() {
        ReturnToLobbyText.text = "Returning to the lobby in " + ReturnToLobbyTimer + " seconds...";
        StartCoroutine(BeginCountdown());

        //todo: do logic to determine and update who won the round
        List<string> playerFinishedNames = new List<string>();
        string kingName = "";
        int winnerCount = 0;

        foreach (PlayerData pData in ServerGameNetPortal.Instance.clientData.Values) { 
            if (pData.IsKing) {
                kingName = pData.PlayerName;
            } else { 
                if (pData.Finished) {
                    playerFinishedNames.Add(pData.PlayerName);
                    winnerCount++;
                }
            }
        }

        if (winnerCount > 0) { 
            // Runners win
            switch(winnerCount) {
                case 1:
                    HeaderText.text = "And the winner is the Runner " + playerFinishedNames[0] + "!";
                    break;
                case 2:
                    HeaderText.text = "And the winners are the Runners " + playerFinishedNames[0] + " and " + playerFinishedNames[1] + "!";
                    break;
            }
        } else {
            // King wins
            HeaderText.text = "And the winner is King " + kingName + "!";
        }
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
