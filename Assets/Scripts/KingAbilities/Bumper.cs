using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    public float bumpPower = 30;

    // Is called whenever something collides with the bumper
    void OnTriggerEnter(Collider objectHit) {
        if (objectHit.tag == "Player") {//Checks if the other object is the player
            dPlayerMovement playerMovement = objectHit.GetComponent<dPlayerMovement>();

            float DirBumpX = playerMovement.driftVel.x * -1;//Inverts the Player Velocity x
            float DirBumpY = .1f;
            float DirBumpZ = playerMovement.driftVel.z * -1;//Inverts the Player Velocity z

            Vector3 DirBump = new Vector3(DirBumpX, DirBumpY, DirBumpZ);//Creates a direction to launch the player
            DirBump = Vector3.Normalize(DirBump);//Normalizes the vector to be used as a bump direction

            playerMovement.GetHit(DirBump, bumpPower);
        }
    }
}
