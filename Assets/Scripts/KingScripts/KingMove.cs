using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingMove : MonoBehaviour
{
    public float speed = 30.0f;
    public float rotSpeed = 30.0f;

    private Vector3 newPos;
    // Update is called once per frame
    void Update() {
        float translation = Input.GetAxis("HorizontalCam") * speed;
        float rotation = Input.GetAxis("HorizontalCam") * rotSpeed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;
        rotation *= Time.deltaTime;

        // Move translation along the object's z-axis
        if (transform.position.z <= 61 && transform.position.z >= -80) {
            transform.Translate(0, 0, -translation);//Is negated to make the Left arrow go left and the right arraow go right
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (transform.position.z > 61) {
            transform.position = new Vector3(-42, 30, 61);//Keeps them from going too far left
        }
        else if (transform.position.z < -80) {//Once they rech a certain point they begin to cirlce around the mountain (radius of 42, x^2+z^2=42^2)
            transform.Rotate(0, -rotation, 0);// Rotates the player as they move along the circumfurance
            float z = transform.position.z - 0.5f;
            float x = Mathf.Sqrt((42 * 42) - ((z + 80) * (z + 80)));//Add something based off translation to X and calculate the corespondent Z Z = sqrt(42^2-x^2)-80
            newPos = new Vector3(-x, 30, z); // make a Vector3 out of the new X and Z
            transform.position = newPos;//Sets the player's new position on the cirlce
        }
        else {
            Debug.Log("Aw, Beans");
            transform.position = new Vector3(-42, 30, 0);
        }
    }
}
