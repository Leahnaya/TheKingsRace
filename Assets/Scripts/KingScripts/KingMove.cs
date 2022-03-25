using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class KingMove : NetworkBehaviour
{
    public float speed = 240.0f;

    private Vector3 newPos;
    private Vector3 MountCent = new Vector3(-4500, 820, 510);
    private Vector3 KingStrPos = new Vector3(350, 825, 1130);
    private Vector3 KingMontStr = new Vector3(-4500, 825, 1130);
    private Vector3 KingMontEnd = new Vector3(-4500, 825, -110);
    private GameObject Grid;

    void Awake()
    {
        Grid = GameObject.FindGameObjectWithTag("KingGrid");
        transform.position = KingStrPos;
    }

    void Start()
    {
        // Turn on the king grid after a couple seconds.  Allows scripts to grab their reference to it before it is disabled
        StartCoroutine(TurnOffGridAtStartOfMatch());
    }

    IEnumerator TurnOffGridAtStartOfMatch()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        Grid.GetComponent<GridReveal>().GridSwitch(false);
    }

    // Update is called once per frame
    void Update() {

        if (!IsLocalPlayer) { return; }

        float translation = Input.GetAxis("KingMove") * speed;

        // Make it move 10 meters per second instead of 10 meters per frame...
        translation *= Time.deltaTime;

        // Move translation along the object's z-axis
        if (transform.position.x <= KingStrPos.x && transform.position.x >= KingMontStr.x) {
            transform.Translate(translation, 0, 0);
            Grid.GetComponent<GridReveal>().DynGridReveal(transform.position.x, translation); //Makes the grid reveal itself as the King moves
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else if (transform.position.x > KingStrPos.x) {
            transform.position = KingStrPos;//Keeps them from going too far left
        }
        else if (transform.position.x < KingMontStr.x) {//Once they rech a certain point they begin to cirlce around the mountain (radius of 1050, (x+4500)^2+(z-510)^2=620^2)
            float z = transform.position.z;
            z -= translation; //Moves the player's X forward slightly
            if (z > KingMontEnd.z && z <= KingMontStr.z) { //Stops this from breaking the circle's formula
                float x = -Mathf.Sqrt((620 * 620) - ((z - MountCent.z) * (z - MountCent.z))) + MountCent.x; //Snaps the player onto a circle that is around the mountain, so the player orbits it smoothly
                newPos = new Vector3(x, KingMontStr.y, z); //make a Vector3 out of the new X and Z
                transform.position = newPos;//Sets the player's new position on the cirlce
                transform.LookAt(MountCent);//Rotates the player as they move along the circumfurance
            }
            if (z > KingMontStr.z) {//Snaps the player back onto the horizontal track
                transform.position = KingMontStr;
            }
        }
        else {//If somehow the player disappears into the void, resets them
            Debug.Log("Aw, Beans");
            transform.position = KingStrPos;
        }
    }
}
