using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Goo : MonoBehaviour
{
    int Lifetime = 0;

    void FixedUpdate() {
        Lifetime++;
        if(Lifetime == 850) {
            DespawnMyselfServerRPC();
        }
    }

    //Makes the player sliperly whenever they step on the goo
    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Debug.Log("Slippy!");
            GameObject player = other.gameObject;//Turns the collider into a game object
            player.GetComponent<PlayerStats>().Traction = 1.5f;//Make it slippy
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnMyselfServerRPC() {
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
