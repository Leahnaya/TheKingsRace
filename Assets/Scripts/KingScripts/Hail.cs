using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hail : MonoBehaviour
{
    private Rigidbody hail;
    public GameObject shadow;

    private float ShadStSz = 0;
    private float m;
    private GameObject ShadTemp = null;


    void Start() {
        hail = GetComponent<Rigidbody>();//Gets the rigidbody attached to the bolder

        ShadTemp = Instantiate(shadow);//Creates the shadow
        ShadTemp.transform.position = new Vector3(hail.transform.position.x, 0, hail.transform.position.z);//Sets it properly directly under the hail //TODO find ground and set it's y ocordingly
        ShadTemp.transform.localScale = new Vector3(ShadStSz, 0, ShadStSz);//Scales it to the proper start size
        m = (hail.transform.localScale.x - ShadStSz) / 100;//Find the proper rate of growth
    }

    void FixedUpdate() {
        float y = m * (hail.transform.position.y-100) + ShadStSz;//Find the proper current size
        ShadTemp.transform.localScale = new Vector3(y, 0, y);//Scales the shadow to that size
        Destruction();//Sees if it needs to destroy both
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "Player") {
            GameObject player = other.gameObject;//Turns the collider into a game object
            float Power = hail.velocity.magnitude * 10;//Finds the power of the hail by using it's velocity and a scaler
            //Find the dirrection of launch by finding the direction between the two points of the player and the Hail
            float px = player.transform.position.x;
            float pz = player.transform.position.z;
            float hx = hail.transform.position.x;
            float hz = hail.transform.position.z;
            //Direction pointing away from the center of the Hail
            Vector3 Dir = new Vector3(px - hx, 0, pz - hz);
            player.GetComponent<dPlayerMovement>().GetHit(Dir, Power); //Ragdolls the player in the direction of the bolder depending on it's speed
        }
    }

    //Destroys the Hail and its shadow
    void Destruction() {
        if(hail.transform.position.y <= 0) {
            Destroy(ShadTemp);
            Destroy(gameObject);
        }
    }
  }
