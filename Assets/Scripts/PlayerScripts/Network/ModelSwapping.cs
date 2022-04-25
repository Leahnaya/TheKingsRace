using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.Messaging;
using System.Linq;

public class ModelSwapping : NetworkBehaviour {
    


    void Start()
    {
        // If game scene & Local Player
        if (SceneManager.GetActiveScene().buildIndex == 3 && IsLocalPlayer) {
            StartCoroutine(WaitSwapModel());
        }
    }

    IEnumerator WaitSwapModel() {
        yield return new WaitForSecondsRealtime(3f);

        SwapModelServerRPC(gameObject.transform.root.GetComponent<NetworkObject>().OwnerClientId);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwapModelServerRPC(ulong playerID) {
        // Spawn the player
        if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(playerID, out string clientGuid)) {
            if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData)) {
                // Get the players item list
                string itemsAsString = string.Join(",", playerData.pInv.NetworkItemList);

                // Apply to each client
                SwapModelClientRPC(playerID, itemsAsString);
            }
        }
    }

    [ClientRpc]
    private void SwapModelClientRPC(ulong playerID, string itemList) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) { 
            if (player.GetComponent<NetworkObject>().OwnerClientId == playerID) {
                // Player to Update found
                player.GetComponentInChildren<ModelSwapping>().SwapModels(itemList);
            }
        }
    }

    public void SwapModels(string itemsAsString) {
        List<string> itemList = itemsAsString.Split(',').ToList();


    }
}
