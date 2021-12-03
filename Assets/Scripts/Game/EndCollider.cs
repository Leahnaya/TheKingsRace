using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCollider : MonoBehaviour {

    // When player collides with the trigger for the end zone
    private void OnTriggerEnter(Collider other) {
        RunnerFinishedMapServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void RunnerFinishedMapServerRPC(ServerRpcParams serverRpcParams = default) {
        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Find our player first
        foreach (GameObject character in playableCharacters) {
            if (character.GetComponent<NetworkObject>().OwnerClientId == serverRpcParams.Receive.SenderClientId) {
                // Player found

                // Despawn them
                character.GetComponent<NetworkObject>().Despawn(true);

                // Update the player data such that Finished is true
                if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(serverRpcParams.Receive.SenderClientId, out string clientGuid)) {
                    ServerGameNetPortal.Instance.clientData[clientGuid] = new PlayerData(
                        ServerGameNetPortal.Instance.clientData[clientGuid].PlayerName,
                        ServerGameNetPortal.Instance.clientData[clientGuid].ClientId,
                        ServerGameNetPortal.Instance.clientData[clientGuid].IsKing,
                        true
                    );
                }
            }
        }
        
        //todo: Check if all runners are finished
        bool allFinished = true;
        foreach (PlayerData pData in ServerGameNetPortal.Instance.clientData.Values) { 
            // Make sure we don't check the king, since the value of Finished will always be false
            if (pData.IsKing != true) { 
                // If one player isn't finished, then set allFinished to false
                if (pData.Finished != true) {
                    allFinished = false;
                }
            }
        }

        if (allFinished) {
            //todo: if yes -> go to PostGame scene

        } else {
            GameObject[] playableCharactersPostRemove = GameObject.FindGameObjectsWithTag("Player");

            foreach (GameObject character in playableCharactersPostRemove) {
                if (character.name == "PlayerPrefab") {
                    FindObject(character, "PlayerCam").SetActive(true);
                }
            }

            //todo: update playerhud to also say SPECTATING
        }
    }

    public GameObject FindObject(GameObject parent, string name)
    {
        Transform[] trs = parent.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trs)
        {
            if (t.name == name)
            {
                return t.gameObject;
            }
        }
        return null;
    }
}
