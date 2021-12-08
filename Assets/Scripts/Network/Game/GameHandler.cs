using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class GameHandler : NetworkBehaviour
{
    public Transform coundownUI;
    public Transform playerHUD;

    private static GameObject _countdownUI;
    private static GameObject _playerHUD;

    // Start is called before the first frame update
    void Start() {
        StartCoroutine(introCutscene());
    }

    IEnumerator introCutscene() {
        // Get the time to complete the intro camera fly through
        float cameraLerpTime = Camera.main.GetComponent<CPC_CameraPath>().playOnAwakeTime;

        // Wait for that many seconds - allows for time to complete the "cutscene"
        yield return new WaitForSecondsRealtime(cameraLerpTime);

        // Remove the main camera and give camera to local players
        Camera.main.gameObject.SetActive(false);

        // Get all players in the scene - runners and king
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("Player");

        // Iterate over them and if their network object equals the call to the rpc, enable that camera
        GameObject localPlayer = null;
        foreach (GameObject character in playableCharacters) {
            if (character.GetComponent<NetworkObject>().OwnerClientId == NetworkManager.Singleton.LocalClientId) {
                localPlayer = character;

                FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<Camera>().enabled = true;
                FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<AudioListener>().enabled = true;
                FindGameObjectInChildWithTag(character, "PlayerCam").GetComponent<PlayerCam>().enabled = true;
            }
        }

        // Spawn the 3.2.1 coundown object
        if (IsHost) {
            SpawnCountdownServerRpc();
        }

        yield return new WaitForSecondsRealtime(5f);

        // Despawn the 3.2.1 coundown object
        if (IsHost) {
            DespawnCountdownServerRpc();
        }

        // Re-enable the player movement
        if (localPlayer.GetComponentInChildren<PlayerMovement>() != null) {
            localPlayer.GetComponentInChildren<PlayerMovement>().enabled = true;
        }

        // Start the game timer and spawn HUDs
        if (IsHost) {
            SpawnPlayerHUDServerRpc();
        }
    }

    // Only works for 1st generation children
    public static GameObject FindGameObjectInChildWithTag(GameObject parent, string tag) {
        Transform t = parent.transform;

        for (int i = 0; i < t.childCount; i++) { 
            if (t.GetChild(i).gameObject.tag == tag) {
                return t.GetChild(i).gameObject;
            }
        }

        // Couldn't find child with tag
        return null;
    }

    [ServerRpc]
    private void SpawnCountdownServerRpc(ServerRpcParams serverRpcParams = default) {
        _countdownUI = Instantiate(coundownUI, Vector3.zero, Quaternion.identity).gameObject;
        _countdownUI.GetComponent<NetworkObject>().Spawn();
    }

    [ServerRpc]
    private void DespawnCountdownServerRpc(ServerRpcParams serverRpcParams = default) {
        _countdownUI.GetComponent<NetworkObject>().Despawn();
        Destroy(_countdownUI);
    }

    [ServerRpc]
    private void SpawnPlayerHUDServerRpc(ServerRpcParams serverRpcParams = default)
    {
        _playerHUD = Instantiate(playerHUD, Vector3.zero, Quaternion.identity).gameObject;
        _playerHUD.GetComponent<NetworkObject>().Spawn(null, true);
    }
}
