using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class GrapplingHook : NetworkBehaviour
{
   public float maxGrappleDistance = 25;

    public bool isGrappled;
    private int hookPointIndex;
    private GameObject hookPoint;
    private GameObject[] hookPoints;
    private float distance;

    private CharacterController movementController;
    private PlayerMovement playerMovement;
    private PlayerStats pStats;
    [SerializeField] private float ropeLength;
    private float climbRate = 5;

    // Start is called before the first frame update
    void Start()
    {
        isGrappled = false; 
        hookPoints = GameObject.FindGameObjectsWithTag("HookPoint");
        movementController = gameObject.GetComponent<CharacterController>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        pStats = gameObject.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsLocalPlayer) { return; }
        if(pStats.HasGrapple){
            if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) //If grapple button is hit
            {
                if (!isGrappled) //If we are not grappling
                {
                    hookPointIndex = FindHookPoint(); //Find the nearest hook point within max distance
                    if (hookPointIndex != -1) //If there is a hookpoint
                    {
                        hookPoint = hookPoints[hookPointIndex]; //The point we are grappling from
                        ropeLength = Vector3.Distance(gameObject.transform.position, hookPoint.transform.position) + 0.5f;
                        isGrappled = true; //toggle grappling state
                    }
                } 
                else //Else we are grappling
                {
                    //physics tear down?
                    isGrappled = false; //toggle grappling state to release
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (!IsLocalPlayer) { return; }
        if (isGrappled)
        {
            Debug.DrawRay(gameObject.transform.position, (hookPoint.transform.position - gameObject.transform.position)); //Visual of line

            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4)) //Extend hook
            {
                ropeLength += climbRate * Time.deltaTime;
                if (ropeLength > maxGrappleDistance)
                {
                    ropeLength = maxGrappleDistance;
                }
                //Debug.Log(ropeLength.ToString());
            }
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.JoystickButton5)) // Retract Hook
            {
                ropeLength -= climbRate * Time.deltaTime;
                if (ropeLength < 5)
                {
                    ropeLength = 5;
                }
                //Debug.Log(ropeLength.ToString());
            }
            //Do grappling physics based on hookPoint
            if (Vector3.Distance(gameObject.transform.position, hookPoint.transform.position) > ropeLength )
            {
                //Impact Based
                Debug.Log("y");
                playerMovement.g = 2;
                movementController.Move((hookPoint.transform.position - gameObject.transform.position) * Time.deltaTime);

            }
        }
        if(playerMovement.GetJumpPressed()) isGrappled = false;
    }

    int FindHookPoint()
    {
        float least = maxGrappleDistance;
        int index = -1;
        for(int i = 0; i<hookPoints.Length; i++)
        {
            distance = Vector3.Distance(gameObject.transform.position, hookPoints[i].transform.position);
            if (distance <= least)
            {
                index = i;
            }
        }
        return index;
    }
    ///////////Hookshot Concept
    // public float maxGrappleDistance = 25;

    // public bool isGrappled;
    // private Vector3 hookPoint;
    // private float distance;
    // public float propelPower = 100;

    // private CharacterController movementController;
    // private dPlayerMovement playerMovement;
    // private PlayerStats pStats;
    // private Camera cam;
    // private Vector3 forwardHookLerp;
    // private Vector3 upwardsHookLerp;

    // // Start is called before the first frame update
    // void Start()
    // {
    //     isGrappled = false;
    //     movementController = gameObject.GetComponent<CharacterController>();
    //     playerMovement = gameObject.GetComponent<dPlayerMovement>();
    //     pStats = gameObject.GetComponent<PlayerStats>();
    //     cam = playerMovement.cam;
    // }

    // private void FixedUpdate()
    // {
    //     getGrapplePoint();
    //     //Debug.Log((hookPoint.transform.position - transform.position).normalized);
    //     if (isGrappled && !Input.GetKeyDown(KeyCode.E))
    //     {
    //         //playerMovement.AddImpact((hookPoint - transform.position), propelPower);
    //         forwardHookLerp = Vector3.Lerp(forwardHookLerp, transform.forward * 50, .03f);
    //         movementController.Move(forwardHookLerp * Time.deltaTime);

    //         upwardsHookLerp = Vector3.Lerp(upwardsHookLerp, (hookPoint - transform.position).normalized * Vector3.Distance(transform.position, hookPoint) * propelPower, .03f);
    //         movementController.Move(upwardsHookLerp * Time.deltaTime);
    //         isGrappled = false;
    //     }
    // }

    // private void getGrapplePoint(){
    //     //if E or left face gamepad button is pressed is slightly pressed
    //     if (Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) //If grapple button is hit
    //     {
    //         if (!isGrappled) //If we are not grappling
    //         {
    //             if(Physics.Raycast(cam.transform.position, cam.transform.forward, out RaycastHit raycastHit)){
    //                 if(raycastHit.collider.gameObject.transform.tag == "HookPoint"){
    //                     isGrappled = true;
    //                     hookPoint = new Vector3 (raycastHit.point.x,raycastHit.point.y,raycastHit.point.z);
    //                 }
    //             }
    //         }
    //     }
    // }
}
