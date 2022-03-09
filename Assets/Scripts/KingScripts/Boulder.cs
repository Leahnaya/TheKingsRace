using MLAPI;
using MLAPI.Messaging;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : NetworkBehaviour
{
    private Rigidbody boulder;
    public float bumpPower = 70;

    void Start() {
       boulder = GetComponent<Rigidbody>();//Gets the rigidbody attached to the bolder
    }

    void OnTriggerEnter(Collider objectHit) {
        if (objectHit.gameObject.tag == "ArcherTarget") {
                    
            Debug.Log("Bump");
            MoveStateManager playerMovement = objectHit.GetComponent<MoveStateManager>();

            float DirBumpX = playerMovement.driftVel.x * -1;//Inverts the Player Velocity x
            float DirBumpY = .1f;
            float DirBumpZ = playerMovement.driftVel.z * -1;//Inverts the Player Velocity z

            Vector3 DirBump = new Vector3(DirBumpX, DirBumpY, DirBumpZ);//Creates a direction to launch the player
            DirBump = Vector3.Normalize(DirBump);//Normalizes the vector to be used as a bump direction

            playerMovement.GetHit(DirBump, bumpPower);
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
