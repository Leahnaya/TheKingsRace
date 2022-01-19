using MLAPI;
using MLAPI.Exceptions;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pitfall : NetworkBehaviour
{
    [SerializeField] private Vector3 RespawnPoint; //Where player will respawn to, set in GUI

    public Transform runnerPrefab;

    private GameObject _runner;

    private bool cooldown = false;

    private void OnTriggerEnter(Collider other) {
        if (cooldown == false && other.transform.parent.gameObject.tag == "Player" && other.GetType() != typeof(BoxCollider) && other.gameObject.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            RespawnPlayerServerRPC(other.gameObject.transform.parent.gameObject.GetComponent<NetworkObject>().OwnerClientId);
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
                        _runner = Instantiate(runnerPrefab, RespawnPoint, Quaternion.Euler(0, -90, 0)).gameObject;
                        _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, null, true);

                        // Turn on camera if the player is the host
                        if (NetworkManager.Singleton.LocalClientId == clientID)
                        {
                            // Recreate Inventory (host)
                            _runner.GetComponentInChildren<PlayerInventory>().UpdateEquips(playerData.pInv.NetworkItemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);

                            GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<Camera>().enabled = true;
                            GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                            GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<PlayerCam>().enabled = true;

                            _runner.GetComponentInChildren<PlayerMovement>().enabled = true;
                        }
                        else
                        {
                            // Spawn via RPC on the server
                            string itemsAsString = string.Join(",", playerData.pInv.NetworkItemList);
                            StartCoroutine(SpawnClient(clientID, itemsAsString));
                        }
                    }
                }
            }
        }
    }

    // The client respawning needs a slight delay to allow for the spawn to properly sync up
    IEnumerator SpawnClient(ulong clientID, string itemsAsString)
    {
        yield return new WaitForSecondsRealtime(1f);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        SpawnPlayerClientRpc(clientID, itemsAsString, clientRpcParams);
    }

    // Spawn in each player
    [ClientRpc]
    public void SpawnPlayerClientRpc(ulong clientId, string itemsAsString, ClientRpcParams clientRpcParams = default)
    {

        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playableCharacters)
        {
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                // Rebuild inventory
                List<string> itemList = itemsAsString.Split(',').ToList();
                character.GetComponentInChildren<PlayerInventory>().UpdateEquips(itemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);

                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<Camera>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<PlayerCam>().enabled = true;

                character.GetComponentInChildren<PlayerMovement>().enabled = true;
            }
        }
    }
}
