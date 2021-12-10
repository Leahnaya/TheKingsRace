using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class dKickController : NetworkBehaviour
{
    //Important, may want to change blink code to only work for trigger collider 
    // so that the player doesn't teleport to their leg
    private GameObject leg;
    private bool isKicking = false;
    //slightly bad practice, when merging find a better work around
    private bool isDiveKicking = false;
    private CharacterController characterController;
    public dPlayerMovement pMove;
    public PlayerStats pStats;

    void Start(){
        pStats = GetComponent<PlayerStats>();
        pMove = GetComponent<dPlayerMovement>();
        characterController = this.gameObject.GetComponent<CharacterController>();
        leg = transform.GetChild(0).gameObject;
        leg.SetActive(false);
    }

    void Update(){
        Kick();
    }

    void Kick(){
        //Note: when we merge this into PlayerMovement, we may want to change isgrounded to our 
        //custom is grounded
        //If F is pressed or gamepad right trigger is pulled
        if ((Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0) && isKicking == false && pMove.isGrounded == false)
        {
            Debug.Log("dive");
            // if kicking in air, kick until grounded (maybe add some foward momentum if needeD)
            isKicking = true;
            isDiveKicking = true;
            leg.SetActive(true);
        }
        //otherwise do ground kick for .3 seconds
        else if ((Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0) && isKicking == false){
            StartCoroutine(Kicking(.3f));
        }

        //once dive kick touches ground, set back to normal state
        if (pMove.isGrounded == true && isDiveKicking == true)
        {
            isDiveKicking = false;
            isKicking = false;
            leg.SetActive(false);

        }
    }

    private IEnumerator Kicking(float waitTime){
        isKicking = true;
        leg.SetActive(true);
        yield return new WaitForSeconds(waitTime);
        isKicking = false;
        leg.SetActive(false);

    }

    private void OnCollisionEnter(Collision collision)
    {
        //if (!IsLocalPlayer) { return; }
        Collider myCollider = collision.contacts[0].thisCollider;
        if (collision.transform.CompareTag("kickable") && myCollider == leg.GetComponent<Collider>()){
            if(collision.gameObject.GetComponent<Rigidbody>().isKinematic == true){
                collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            Vector3 direction = this.transform.forward;
            Debug.Log(direction);
            collision.rigidbody.AddForce(direction * pStats.KickPow, ForceMode.Impulse);
        }
        if (collision.transform.CompareTag("destroyable") && myCollider == leg.GetComponent<Collider>()){
            collision.transform.gameObject.GetComponent<BreakableBlock>().damage(pStats.KickPow);
        }
    }

}
