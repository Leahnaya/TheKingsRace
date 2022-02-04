using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime : MonoBehaviour
{
    float MoveDir;
    int GooTimer = 0;
    Vector3 offset = new Vector3(0.0f, 0.4f, 0.0f);
    public GameObject Goo;
    public GameObject Folder;
    public int StartPos = -29;
    public int EndPos = -57;
    // Start is called before the first frame update
    void Start()
    {
        MoveDir = Time.deltaTime;
    }

    // FixedUpdate is called once per .02 seconds (or 50 times a second)
    void FixedUpdate()
    {
        //create goo underneath them slime
        if (GooTimer == 6) {
            GameObject GooTrail = null;
            GooTrail = Instantiate(Goo, transform.position - offset, transform.rotation);
            GooTrail.transform.parent = Folder.transform;
            GooTimer = 0;
        }
        //attempt to move forward(Raycast for objects, how to make sure it didn't fall off a cliff?)
        if (transform.position.z >= StartPos || transform.position.z <= EndPos) {//Rough way of keeping turning it around when it reaches an end
            MoveDir = -MoveDir;
        }
        //if can't Turn around 180 degrees
        //otherwise actually move forward
        transform.Translate(0, 0, MoveDir);
        GooTimer++;
    }

    void OnTriggerStay(Collider other) {
        //slow down player
        GameObject player = other.gameObject;//Turns the collider into a game object
        //Get the Player's speed
        float CurVel = player.GetComponent<PlayerStats>().CurVel;
        player.GetComponent<PlayerStats>().CurVel = CurVel - (CurVel/10);
        //Cut it in half?
    }

}
