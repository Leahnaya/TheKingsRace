using MLAPI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : NetworkBehaviour
{
    [SerializeField] private int raceTimer;
    public TMP_Text countdown_text;

    public GameObject RunnerHUD;
    public GameObject KingHUD;

    // Start is called before the first frame update
    void Start() {
        countdown_text.text = "Time Remaining: 00:00";
        StartCoroutine(RaceTimeCountdown());

        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Find our player and enable their hud respectively
        foreach (GameObject character in playableCharacters) {
            if (character.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
                if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(NetworkManager.Singleton.LocalClientId, out string clientGuid)) {
                    if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData)) {
                        if (playerData.IsKing) {
                            KingHUD.SetActive(true);
                        } else {
                            RunnerHUD.SetActive(true);
                        }
                    }
                }
            }
        }
    }

    IEnumerator RaceTimeCountdown() {
        for (int i = raceTimer; i > 0; i--) {
            int minutes = i / 60;
            int seconds = i % 60;

            countdown_text.text = "Time Remaining: " + minutes.ToString("D2") + ":" + seconds.ToString("D2");

            yield return new WaitForSeconds(1f);
        }

        // Timer is over

        // Only have the host call the server RPC to it is only done once
        if (IsHost) {
            //TODO: WRITE WIN/LOSS CONDITIONAL
            //TODO: Call server RPC for win loss stuff
        }
    }
}
