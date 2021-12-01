using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    private Rigidbody boulder;

    void Start() {
       boulder = GetComponent<Rigidbody>();//Gets the rigidbody attached to the bolder
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            Debug.Log("Crunch");
            GameObject player = other.gameObject;//Turns the collider into a game object
            Vector3 Dir = boulder.velocity;//Finds the bolder's velocity
            float Power = boulder.velocity.magnitude * 250;//Finds the power of the boulder by using it's velocity and a scaler
            Dir = Vector3.Normalize(Dir);//Normalizes the vector to be used as a knockback direction
            player.GetComponent<dPlayerMovement>().getHit(Dir, Power); //Ragdolls the player in the direction of the bolder depending on it's speed
        }
    }

    //Coppied from player for easy Boulder Testing/Demonstration
    void FixedUpdate() {
        Respawn();
    }

    private void Respawn()
    {
        if (transform.position.y < -1) {
            transform.position = new Vector3(74.67f, 34.68f, 7.15f);
        }
    }
}
