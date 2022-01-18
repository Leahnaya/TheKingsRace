using MLAPI;
using MLAPI.Exceptions;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
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

        Debug.LogError("# of players: " + playableCharacters.Length);

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
                    Debug.Log("Exception");
                    return;
                }
                
                // Spawn the player
                if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(clientID, out string clientGuid))
                {
                    if (ServerGameNetPortal.Instance.clientData.TryGetValue(clientGuid, out PlayerData playerData))
                    {
                        // Spawn as player
                        _runner = Instantiate(runnerPrefab, RespawnPoint, Quaternion.Euler(0, -90, 0)).gameObject;
                        //Recreate Inventory
                        // FOLLOWING LINE WILL NEED TO BE UPDATED SINCE THIS WILL ONLY WORK FOR THE HOST AND NOT THE CONNECTED CLIENTS
                        _runner.GetComponentInChildren<PlayerInventory>().UpdateEquips(playerData.pInv.NetworkItemList, this.gameObject.GetComponent<InventoryManager>().ItemDict);
                        _runner.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientID, null, true);

                        // Turn on camera if the player is the host
                        if (NetworkManager.Singleton.LocalClientId == clientID)
                        {
                            GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<Camera>().enabled = true;
                            GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                            GameHandler.FindGameObjectInChildWithTag(_runner, "PlayerCam").GetComponent<PlayerCam>().enabled = true;

                            _runner.GetComponentInChildren<PlayerMovement>().enabled = true;
                        }
                        else
                        {
                            // Spawn via RPC on the server
                            StartCoroutine(SpawnClient(clientID));
                        }
                    }
                }
            }
        }
    }

    // The client respawning needs a slight delay to allow for the spawn to properly sync up
    IEnumerator SpawnClient(ulong clientID)
    {
        yield return new WaitForSecondsRealtime(1f);

        ClientRpcParams clientRpcParams = new ClientRpcParams
        {
            Send = new ClientRpcSendParams
            {
                TargetClientIds = new ulong[] { clientID }
            }
        };

        SpawnPlayerClientRpc(clientID, clientRpcParams);
    }

    // Spawn in each player
    [ClientRpc]
    public void SpawnPlayerClientRpc(ulong clientId, ClientRpcParams clientRpcParams = default)
    {

        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playableCharacters)
        {
            if (character.GetComponent<NetworkObject>().OwnerClientId == clientId)
            {
                Debug.LogError("Found");
                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<Camera>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<PlayerCam>().enabled = true;

                character.GetComponentInChildren<PlayerMovement>().enabled = true;
            }
            else
            {
                Debug.LogError("Failed to find");
            }
        }
    }
}
