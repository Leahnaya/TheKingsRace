using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using MLAPI;
using MLAPI.Messaging;
using System.Linq;

public class ModelSwapping : NetworkBehaviour {

    // Runner Body Parts Refs
    public GameObject[] runnerBodyModifiers;

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

        if (itemList.Count <= 0) { return; }

        foreach (string itemString in itemList) {
            GameObject.FindGameObjectWithTag("GameManager").GetComponent<InventoryManager>().ItemDict.TryGetValue(itemString, out Item refItem);

            if (refItem == null) { continue; }

            switch (refItem.id) {
                //Glider
                case 2:
                    runnerBodyModifiers[1].SetActive(true);
                    //glider.SetActive(true);
                    break;
                //grapple
                case 3:
                    runnerBodyModifiers[11].SetActive(true);
                    runnerBodyModifiers[10].SetActive(false);
                    break;
                //nitro
                case 4:
                    runnerBodyModifiers[0].SetActive(true);
                    break;
                //roller skates
                case 5:
                    runnerBodyModifiers[12].SetActive(true);
                    runnerBodyModifiers[13].SetActive(true);
                    break;
                //springs
                case 6:
                    runnerBodyModifiers[4].SetActive(true);
                    runnerBodyModifiers[5].SetActive(true);

                    runnerBodyModifiers[2].SetActive(false);
                    runnerBodyModifiers[3].SetActive(false);
                    break;
                //tripple jump
                case 7:
                    runnerBodyModifiers[16].SetActive(true);
                    runnerBodyModifiers[17].SetActive(true);

                    runnerBodyModifiers[14].SetActive(false);
                    runnerBodyModifiers[15].SetActive(false);
                    break;
                //wallrun
                case 8:
                    runnerBodyModifiers[8].SetActive(true);
                    runnerBodyModifiers[9].SetActive(true);

                    runnerBodyModifiers[6].SetActive(false);
                    runnerBodyModifiers[7].SetActive(false);
                    break;

            }
        }
    }
}
