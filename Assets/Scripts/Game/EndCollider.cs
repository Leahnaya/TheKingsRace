using MLAPI;
using MLAPI.Messaging;
using MLAPI.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndCollider : NetworkBehaviour {

    // When player collides with the trigger for the end zone
    private void OnTriggerEnter(Collider other) {
        if (other.transform.gameObject.tag == "PlayerTrigger" && other.gameObject.transform.root.gameObject.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
            RunnerFinishedMapServerRPC();
        }
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
            // Go to the post game screen
            NetworkSceneManager.SwitchScene("PostGame");
        } else {
            // Loop over all characters
            foreach (GameObject character in playableCharacters) {
                // Make sure we find the characters that aren't the one that just finished (subsequently calling the rpc)
                if (character.GetComponent<NetworkObject>().OwnerClientId != serverRpcParams.Receive.SenderClientId) {
                    // Then grab their GUID
                    if (ServerGameNetPortal.Instance.clientIdToGuid.TryGetValue(character.GetComponent<NetworkObject>().OwnerClientId, out string clientGuid)) {
                        // To verify they aren't the king
                        if (ServerGameNetPortal.Instance.clientData[clientGuid].IsKing != true) {
                            // Then call a client rpc to the finished player to enable the camera locally
                            ClientRpcParams clientRpcParams = new ClientRpcParams {
                                Send = new ClientRpcSendParams {
                                    TargetClientIds = new ulong[] { serverRpcParams.Receive.SenderClientId }
                                }
                            };

                            EnableSpectatorCameraClientRPC(character.GetComponent<NetworkObject>().OwnerClientId, clientRpcParams);
                        }
                    }
                }
            }
        }
    }

    [ClientRpc]
    private void EnableSpectatorCameraClientRPC(ulong otherRunnerClientID, ClientRpcParams clientRpcParams = default) {
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject character in playableCharacters) {
            if (character.GetComponent<NetworkObject>().OwnerClientId == otherRunnerClientID) {
                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<Camera>().enabled = true;
                GameHandler.FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<AudioListener>().enabled = true;
            }
        }

        GameObject.FindGameObjectWithTag("RunnerHUD").GetComponent<PlayerHUD>().spectating_text.text = "SPECTATING";
    }
}
