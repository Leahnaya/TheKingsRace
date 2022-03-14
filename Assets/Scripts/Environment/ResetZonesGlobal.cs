using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class ResetZonesGlobal : MonoBehaviour {

    private bool cooldown = false;

    public int RespawnZone;

    private void OnTriggerEnter(Collider other) {
        if (cooldown == false && other.transform.gameObject.tag == "PlayerTrigger" && other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            SetClientRespawnZoneServerRPC(other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId, RespawnZone);
            cooldown = true;
            StartCoroutine(CoolItDown());
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetClientRespawnZoneServerRPC(ulong clientID, int RespawnZone) {
        SetClientRespawnZoneClientRPC(clientID, RespawnZone);
    }

    [ClientRpc]
    private void SetClientRespawnZoneClientRPC(ulong clientID, int RespawnZone) {
        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Find our player first
        foreach (GameObject character in playableCharacters) {
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientID) {
                // Set their zone
                if (character.GetComponent<ResetZones>() != null) {
                    character.GetComponent<ResetZones>().SetCurrentZone(RespawnZone);
                }
            }
        }
    }

    IEnumerator CoolItDown() {
        yield return new WaitForSecondsRealtime(1f);
        cooldown = false;
    }
}
