using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goo : MonoBehaviour
{
    int Lifetime = 0;

    void FixedUpdate() {
        Lifetime++;
        if(Lifetime == 950) {
            Destruction();
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

    void Destruction() {
        Destroy(gameObject);
    }
}
