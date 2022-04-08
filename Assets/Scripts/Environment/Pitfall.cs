using MLAPI;
using MLAPI.Exceptions;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pitfall : NetworkBehaviour
{
    [SerializeField] private Transform RespawnPoint; //Where player will respawn to, set in GUI

    public Transform runnerPrefab;

    private GameObject _runner;

    private bool cooldown = false;

    private void OnTriggerEnter(Collider other) {
        if (cooldown == false && other.transform.gameObject.tag == "PlayerTrigger" && other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            RespawnPlayerServerRPC(other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId);
            cooldown = true;
            StartCoroutine(CoolItDown());
        }
    }

    IEnumerator CoolItDown()
    {
        yield return new WaitForSecondsRealtime(1f);
        cooldown = false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void RespawnPlayerServerRPC(ulong clientID, ServerRpcParams serverRpcParams = default)
    {

        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Find our player first
        foreach (GameObject character in playableCharacters)
        {
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientID)
            {
                // Player found

                // Despawn them
                try
                {
                    character.GetComponent<NetworkObject>().Despawn(true);
                } catch (SpawnStateException e) {
                    Debug.LogError("Spawn State Exception Exception:");
                    Debug.LogError(e);
                    return;
                }
                
                // Spawn the player
                if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(clientID, out string clientGuid))
                {
                    if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData))
                    {
                        // Spawn as player
                        _runner = Instantiate(runnerPrefab, RespawnPoint.position, Quaternion.Euler(0, -90, 0)).gameObject;
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
        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        UIRespawningClientRpc(clientID, clientRpcParams);

        // 3 Second Respawn Delay
        yield return new WaitForSecondsRealtime(3f);

        // Now actually respawn the player
        clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        SpawnPlayerClientRpc(clientID, itemsAsString, clientRpcParams);
    }

    [ClientRpc]
    private void UIRespawningClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default) {
        GameObject.FindGameObjectWithTag("RunnerHUD").GetComponent<PlayerHUD>().setRespawnPanelVisibility(true);
        GameObject.FindGameObjectWithTag("RunnerHUD").GetComponent<PlayerHUD>().countdown_text.enabled = false;
    }

    // Spawn in each player
    [ClientRpc]
    public void SpawnPlayerClientRpc(ulong clientId, string itemsAsString, ClientRpcParams clientRpcParams = default) {

        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playableCharacters)
        {
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
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
