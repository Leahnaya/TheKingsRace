using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingMove : MonoBehaviour
{
    public float speed = 30.0f;

    private Vector3 newPos;
    private Vector3 MountCent = new Vector3(0, 30, -80);
    private bool RotStr = false;
    // Update is called once per frame
    void Update() {
        float translation = Input.GetAxis("HorizontalCam") * speed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;

        // Move translation along the object's z-axis
        if (transform.position.z <= 61 && transform.position.z >= -80) {
            transform.Translate(translation, 0, 0);//Is negated to make the Left arrow go left and the right arraow go right
            //transform.position = new Vector3(-42, 30, transform.position.z);
            transform.rotation = Quaternion.Euler(0, 90, 0);
            RotStr = false;
        }
        else if (transform.position.z > 61) {
            transform.position = new Vector3(-42, 30, 61);//Keeps them from going too far left
        }
        else if (transform.position.z < -80) {//Once they rech a certain point they begin to cirlce around the mountain (radius of 42, x^2+(z+80)^2=42^2)
            float x = transform.position.x;
            x += translation; //Moves the player's X forward slightly
            float z = -Mathf.Sqrt((42 * 42) - (x * x)) - 80; //Snaps the player onto a circle that is around the mountain, so the player orbits it smoothly
            newPos = new Vector3(x, 30, z); //make a Vector3 out of the new X and Z
            transform.position = newPos;//Sets the player's new position on the cirlce
            transform.LookAt(MountCent);//Rotates the player as they move along the circumfurance
            /*if (RotStr == false) {//Rotates the player by 90 degrees once, to make LookAt work properly
                transform.Rotate(0f, -90f, 0f);
                RotStr = true;
            }*/
            if (x < -42) {//Snaps the player back onto the horizontal track
                transform.position = new Vector3(-42, 30, -80);
            }
        }
        else {//If somehow the player disappears into the void, resets them
            Debug.Log("Aw, Beans");
            transform.position = new Vector3(-42, 30, 0);
        }
    }
}
