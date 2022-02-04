using System.Collections;
using System.Collections.Generic;
using MLAPI;
using MLAPI.Messaging;
using UnityEngine;

public class KickController : NetworkBehaviour
{
    private GameObject leg;
    private GameObject legHitbox;
    private bool isKicking = false;
    //slightly bad practice, when merging find a better work around
    private bool isDiveKicking = false;
    private CharacterController characterController;
    public PlayerMovement pMove;
    public PlayerStats pStats;
    private float legRotation = 0;

    void Start(){
        pStats = GetComponent<PlayerStats>();
        pMove = GetComponent<PlayerMovement>();
        characterController = this.gameObject.GetComponent<CharacterController>();
        leg = transform.Find("Leg").gameObject;
        legHitbox = leg.transform.Find("LegHitbox").gameObject;
        leg.SetActive(false);
    }

    void Update(){
        Kick();
    }

    void Kick(){
        if (!IsLocalPlayer) { return; }
        //Note: when we merge this into PlayerMovement, we may want to change isgrounded to our 
        //custom is grounded
        //If F is pressed or gamepad right trigger is pulled
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0) && isKicking == false && pMove.isGrounded == false && pMove.isSliding==false)
        {
            Debug.Log("dive");
            // if kicking in air, kick until grounded (maybe add some foward momentum if needeD)
            isKicking = true;
            isDiveKicking = true;
            leg.SetActive(true);
        }
        //otherwise do ground kick for .3 seconds
        else if ((Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0) && isKicking == false && pMove.isSliding==false){
            StartCoroutine(Kicking(1f));
        }

        //once dive kick touches ground, set back to normal state
        if (pMove.isGrounded == true && isDiveKicking == true)
        {
            isDiveKicking = false;
            isKicking = false;
            legRotation = 0;
            leg.transform.eulerAngles = new Vector3(legRotation, leg.transform.eulerAngles.y, leg.transform.eulerAngles.z);
            leg.SetActive(false);

        }

        if(isKicking){
            RotateLeg();
            characterController.Move(new Vector3(.0015f,0,0));
        }
    }

    private IEnumerator Kicking(float waitTime){
        isKicking = true;
        leg.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        isKicking = false;
        legRotation = 0;
        leg.transform.eulerAngles = new Vector3(legRotation, leg.transform.eulerAngles.y, leg.transform.eulerAngles.z);
        legHitbox.GetComponent<Collider>().isTrigger = false;
        leg.SetActive(false);

    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) { return; }
        Collider myCollider = collision.contacts[0].thisCollider;

        // Kickable items must be handled through the server since they need to modify the NetworkTransform
        if (collision.transform.CompareTag("kickable") && myCollider == legHitbox.GetComponent<Collider>()) {
            Vector3 direction = this.transform.forward;
            ulong prefabHash = collision.gameObject.GetComponent<NetworkObject>().PrefabHash;
            ApplyKickServerRPC(direction, prefabHash);
        }

        if (collision.transform.CompareTag("destroyable") && myCollider == legHitbox.GetComponent<Collider>()){
            collision.transform.gameObject.GetComponent<BreakableBlock>().damage(pStats.KickPow);
        }
    }

    private void RotateLeg(){
        if(legRotation > -90){
            leg.transform.eulerAngles = new Vector3(legRotation, leg.transform.eulerAngles.y, leg.transform.eulerAngles.z);
            legRotation -= 20;
        }
        else{
            legRotation = -90;
        }
        
    }


    [ServerRpc(RequireOwnership = false)]
    private void ApplyKickServerRPC(Vector3 direction, ulong prefabHash) {
        GameObject[] kickables = GameObject.FindGameObjectsWithTag("kickable");

        foreach (GameObject kickedItem in kickables) { 
            // First check to make sure this is the item we kicked
            if (kickedItem.GetComponent<NetworkObject>() != null && kickedItem.GetComponent<NetworkObject>().PrefabHash == prefabHash) {
                // First turn off kinematic
                if (kickedItem.gameObject.GetComponent<Rigidbody>().isKinematic == true) {
                    kickedItem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                }

                // Then apply the force
                kickedItem.gameObject.GetComponent<Rigidbody>().AddForce(direction * pStats.KickPow, ForceMode.Impulse);

                // Then return since we only kicked one item and don't need to check the remainder of the items
                return;
            }
        }
    }
}
