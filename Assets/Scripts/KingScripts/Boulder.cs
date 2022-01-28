using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : NetworkBehaviour
{
    private Rigidbody boulder;

    void Start() {
       boulder = GetComponent<Rigidbody>();//Gets the rigidbody attached to the bolder
    }

    void OnTriggerEnter(Collider other) {
        if (other.transform.parent.gameObject.tag == "Player") {
            GameObject player = other.gameObject;//Turns the collider into a game object
            Vector3 Dir = boulder.velocity;//Finds the bolder's velocity
            float Power = boulder.velocity.magnitude;// * 250;//Finds the power of the boulder by using it's velocity and a scaler
            Dir = Vector3.Normalize(Dir);//Normalizes the vector to be used as a knockback direction
            //Knock the player a little to the side
            Dir.z += Random.Range(-0.25f, 0.25f);
            player.GetComponent<PlayerMovement>().GetHit(Dir, Power); //Ragdolls the player in the direction of the bolder depending on it's speed
        }
    }

    public void StartCountdown(int time, Vector3 spawnForce)
    {
        StartCoroutine(DespawnCounter(time));
        AddForceServerRPC(spawnForce);
    }

    [ServerRpc]
    private void AddForceServerRPC(Vector3 force) {
        this.gameObject.GetComponent<Rigidbody>().AddForce(force);
    }

    IEnumerator DespawnCounter(int time) {
        for (int i = time; i > 0; i--) {
            yield return new WaitForSecondsRealtime(1f);
        }

        // Time's up - Despawn us
        DespawnBoulderServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnBoulderServerRPC() {
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
