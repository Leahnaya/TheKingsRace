using MLAPI;
using MLAPI.Messaging;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostGameUI : NetworkBehaviour {

    [SerializeField] private TMP_Text ReturnToLobbyText;
    [SerializeField] private int ReturnToLobbyTimer;

    void Start() {
        ReturnToLobbyText.text = "Returning to the lobby in " + ReturnToLobbyTimer + " seconds...";
        StartCoroutine(BeginCountdown());

        //todo: do logic to determine and update who won the round
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
