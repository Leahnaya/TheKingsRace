using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Slime : NetworkBehaviour
{
    float MoveSpd;
    int GooTimer = 0;
    int Lifetime = 0;
    bool GooGo = false;
    Vector3 GooOffset = new Vector3(0.0f, -0.1f, 0.0f);
    Vector3 RayOffset = new Vector3(0.0f, -0.5f, 0.0f);
    Vector3 MoveDir = new Vector3(0, 0, 1);
    RaycastHit hit;
    public GameObject Goo;
    [SerializeField] private LayerMask LayerMask;
    // Start is called before the first frame update
    void Start()
    {
        MoveSpd = 9*Time.deltaTime;
    }

    // FixedUpdate is called once per .02 seconds (or 50 times a second)
    void FixedUpdate()
    {
        if (GooGo == true) {//Makes it make goo and move only if it's already placed
            //create goo underneath them slime
            if (GooTimer == 6) {
                GameObject GooTrail = null;
                GooTrail = Instantiate(Goo, transform.position - GooOffset, transform.rotation);
                GooTrail.GetComponent<NetworkObject>().Spawn(null, true);
                GooTimer = 0;
            }
            //attempt to move forward(Raycast infront for objects, and also raycast down, to make sure there's still ground underneath it)
            if (Physics.Raycast(transform.position, transform.TransformDirection(MoveDir), out hit, 2f, LayerMask) || Physics.Raycast(transform.position + RayOffset, transform.TransformDirection(Vector3.down), out hit, 50f, LayerMask)) {//turns it around when it hits something or is going to go over a pit//TODO hit more than terrain
                MoveDir = -MoveDir;
            }
            transform.Translate(MoveSpd*MoveDir);//moving forward
            GooTimer++;

            Lifetime++;
            if (Lifetime == 3000) {
                DespawnMyselfServerRPC();
            }
        }
    }

    //TODO: FIX THIS
    void OnTriggerStay(Collider other) {
        if (other.tag == "Player") {
            //slow down player
            GameObject player = other.gameObject;//Turns the collider into a game object
            float CurVel = player.GetComponent<PlayerStats>().CurVel;//Get the Player's speed
            player.GetComponent<PlayerStats>().CurVel = CurVel - (CurVel / 10);
            //Cut it in half?
        }
    }

    public void GooStart(int SlimeDir) {
        GooGo = true;
        switch (SlimeDir) {//Sets the slime's initial direction properly
            case 0:
                MoveDir = Vector3.back;
                break;
            case 1:
                MoveDir = Vector3.left;
                break;
            case 2:
                MoveDir = Vector3.forward;
                break;
            case 3:
                MoveDir = Vector3.right;
                break;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnMyselfServerRPC() {
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
