using MLAPI;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : NetworkBehaviour
{
    [SerializeField] private int raceTimer;
    public TMP_Text countdown_text;

    public TMP_Text spectating_text;

    // Start is called before the first frame update
    void Start() {
        countdown_text.text = "Time Remaining: 00:00";
        StartCoroutine(RaceTimeCountdown());
    }

    IEnumerator RaceTimeCountdown() {
        for (int i = raceTimer; i >= 0; i--) {
            int minutes = i / 60;
            int seconds = i % 60;

            countdown_text.text = "Time Remaining: " + minutes.ToString("D2") + ":" + seconds.ToString("D2");

            yield return new WaitForSeconds(1f);
        }

        // Timer is over

        // Only have the host call the server RPC to it is only done once
        if (IsHost) {
            EndRoundServerRPC();
        }
    }

    [ServerRpc]
    private void EndRoundServerRPC() {
        NetworkSceneManager.SwitchScene("PostGame");
    }
}
