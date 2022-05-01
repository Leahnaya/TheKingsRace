using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bumper : MonoBehaviour {

    public float bumpPower = 30;
    public AudioSource bumpNoise;
    // Is called whenever something collides with the bumper
    void OnTriggerEnter(Collider objectHit) {
        if (objectHit.gameObject.tag == "ArcherTarget") {//Checks if the other object is the player
        
            //Debug.Log("Bump");
            MoveStateManager playerMovement = objectHit.GetComponent<MoveStateManager>();

            Vector3 dirBump = objectHit.transform.position - transform.position;

            dirBump.y = .1f;
            if(dirBump.x == 0 && dirBump.z == 0){
                dirBump = new Vector3(1,.1f,1);
            }
            bumpNoise.Play();
            playerMovement.GetHit(dirBump.normalized, bumpPower);
            //
        }
    }
}
