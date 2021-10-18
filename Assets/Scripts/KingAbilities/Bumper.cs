using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    // Is called whenever something collides with the bumper
    void OnTriggerEnter(Collider other) {
        //Debug.Log("Bump!");
        GameObject player = other.gameObject;//Turns the collider into a game object
        Vector3 DirBump = player.GetComponent<PlayerMovement>().vel * -1;//Inverts the Player Velocity
        DirBump = Vector3.Normalize(DirBump);//Normalizes the vector to be used as a bump direction
        player.GetComponent<PlayerMovement>().AddImpact(DirBump, 200); //Launches tthe player directly away from the bumper
    }
}
