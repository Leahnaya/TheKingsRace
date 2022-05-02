using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class HailArea : NetworkBehaviour
{
    // Update is called once per frame
    private float Xmax = 15f;
    private float Xmin = -15f;

    private float Zmax = -85f;
    private float Zmin = -115f;

    private int Ytop = 600;
    int Lifetime = 0;

    public GameObject Hail;

    private GameObject spawnedHail;

    int timer = 0;
    int timeMax = 75;

    RaycastHit hit;
    [SerializeField] private LayerMask LayerMask;
    float height = 0f;
    // FixedUpdate is called once per .02 seconds (or 50 times a second)
    void FixedUpdate()
    {
        if (timer == timeMax) {
            timer = 0;
            //Random diameter 4->8
            float diameter = Random.Range(4, 8);
            float radius = diameter / 2.0f;
            //Random pos X Xmax-radius -> Xmin+radius
            //Random pos Z Zmax-radius -> Zmin+radius

            Vector3 position = new Vector3(Random.Range(Xmin + radius, Xmax - radius), Ytop, Random.Range(Zmin + radius, Zmax - radius));//Finds where the hail will spawn in the air
            if (Physics.Raycast(position, transform.TransformDirection(Vector3.down), out hit, float.MaxValue, LayerMask)) {//Raycasts to find where the ground is
                height = Ytop - hit.distance;
            }
            position = new Vector3(position.x, 100+height, position.z); //find ground and set y occordingly

            SpawnHailServerRPC(diameter, position);
        }
        timer++;

        Lifetime++;
        if (Lifetime == 3000) {
            DespawnMyselfServerRPC();
        }
    }

    // Spawns a singular Hail piece
    [ServerRpc(RequireOwnership = false)]
    void SpawnHailServerRPC(float size, Vector3 pos) {
        spawnedHail = Instantiate(Hail, pos, Quaternion.identity);
        spawnedHail.GetComponent<NetworkObject>().Spawn(null, true);
        ResizeHailClientRPC(spawnedHail.GetComponent<NetworkObject>().NetworkObjectId, size);
    }

    [ClientRpc]
    private void ResizeHailClientRPC(ulong hailID, float size) {
        GameObject[] hails = GameObject.FindGameObjectsWithTag("Hail");

        foreach (GameObject hail in hails) { 
            if (hail.GetComponent<NetworkObject>().NetworkObjectId == hailID) {
                hail.transform.localScale = new Vector3(size, size, size);
            }
        }
    }

    public void SetArea(float LeftBound, float RightBound, float TopBound, float BottomBound) {
        Xmin = LeftBound;
        Xmax = RightBound;
        Zmax = TopBound;
        Zmin = BottomBound;
        float distance = Mathf.Sqrt(Mathf.Pow((Xmax - Xmin), 2) - Mathf.Pow((Zmax - Zmin), 2)); //Finds the size of the box
        //Converts it into a timer for spawing things properly
        if(distance < 0) {
            distance *= -1;
        }
        if(distance <= 60) {
            timeMax = 25;
        }
        else if(distance > 60  && distance <= 250) {
            timeMax = 20;
        }
        else if (distance > 250 && distance <= 500) {
            timeMax = 15;
        }
        else if (distance > 500) {
            timeMax = 10;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnMyselfServerRPC() {
        this.gameObject.GetComponent<NetworkObject>().Despawn(true);
    }
}
