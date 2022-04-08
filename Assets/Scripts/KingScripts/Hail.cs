using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class Hail : NetworkBehaviour
{
    private Rigidbody hail;
    public GameObject shadow;

    private float ShadStSz = 0;
    private float m;
    private GameObject ShadTemp = null;

    ulong shadowID = 0;

    void Start() {
        if (!IsServer) { return; }

        hail = GetComponent<Rigidbody>();//Gets the rigidbody attached to the bolder

        m = hail.transform.localScale.x / 99.9f;//Find the proper rate of growth

        SpawnShadowServerRPC();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnShadowServerRPC() { 
        ShadTemp = Instantiate(shadow, new Vector3(hail.transform.position.x, hail.transform.position.y - 99.9f, hail.transform.position.z), Quaternion.identity);

        ShadTemp.transform.localScale = new Vector3(ShadStSz, 0, ShadStSz);//Scales it to the proper start size

        ShadTemp.GetComponent<NetworkObject>().Spawn(null, true);

        shadowID = ShadTemp.GetComponent<NetworkObject>().NetworkObjectId;
    }

    void FixedUpdate() {
        ScaleShadowServerRPC();//Scales the shadow to that size
        DespawnMyselfServerRPC(shadowID);//Sees if it needs to destroy both
    }

    [ServerRpc(RequireOwnership = false)]
    private void ScaleShadowServerRPC() {
        float y = -m * (hail.transform.position.y - ShadTemp.transform.position.y) + hail.transform.localScale.x;//Find the proper current size
        ScaleShadowClientRPC(shadowID, y);
    }

    [ClientRpc]
    private void ScaleShadowClientRPC(ulong sID, float size) {
        GameObject[] shadows = GameObject.FindGameObjectsWithTag("Shadows");

        foreach (GameObject shad in shadows) {
            if (shad.GetComponent<NetworkObject>().NetworkObjectId == sID) {
                shad.transform.localScale = new Vector3(size, 0, size);
            }
        }
    }

    void OnTriggerEnter(Collider other) {
        if (other.tag == "ArcherTarget") {
            GameObject player = other.gameObject;//Turns the collider into a game object
            float Power = 50;//Finds the power of the hail by using it's velocity and a scaler
            //Find the dirrection of launch by finding the direction between the two points of the player and the Hail
            float px = player.transform.position.x;
            float pz = player.transform.position.z;
            float hx = gameObject.transform.position.x;
            float hz = gameObject.transform.position.z;
            //Direction pointing away from the center of the Hail
            Vector3 Dir = new Vector3(px - hx, 0, pz - hz);
            other.GetComponent<MoveStateManager>().GetHit(Dir, Power);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DespawnMyselfServerRPC(ulong sID) {
        GameObject[] shadows = GameObject.FindGameObjectsWithTag("Shadows");

        foreach (GameObject shad in shadows) {
            if (shad.GetComponent<NetworkObject>().NetworkObjectId == sID) {
                if (hail.transform.position.y <= shad.transform.position.y) {
                    shad.GetComponent<NetworkObject>().Despawn(true);
                    this.gameObject.GetComponent<NetworkObject>().Despawn(true);
                }
            }
        }
    }
}
