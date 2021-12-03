using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    // Is called whenever something collides with the bumper
    void OnTriggerEnter(Collider other) {
        Debug.Log("Bump!");
        if (other.tag == "Player") {//Checks if the other object is the player
            float DirBumpX = other.GetComponent<dPlayerMovement>().vel.x * -1;//Inverts the Player Velocity x
            float DirBumpZ = other.GetComponent<dPlayerMovement>().vel.z * -1;//Inverts the Player Velocity y
            Vector3 DirBump = new Vector3(DirBumpX, .01f, DirBumpZ);//Creates a direction to launch the player
            DirBump = Vector3.Normalize(DirBump);//Normalizes the vector to be used as a bump direction
            other.GetComponent<dPlayerMovement>().GetHit(DirBump, 50); //Launches tthe player directly away from the bumper
        }
    }
}
