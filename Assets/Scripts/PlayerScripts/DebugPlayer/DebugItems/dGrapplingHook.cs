using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class dGrapplingHook : NetworkBehaviour
{
    public float maxGrappleDistance = 25;

    public bool isGrappled;
    private int hookPointIndex;
    private GameObject hookPoint;
    private GameObject[] hookPoints;
    private float distance;

    private CharacterController movementController;
    private dPlayerMovement playerMovement;
    private PlayerStats pStats;
    [SerializeField] private float ropeLength;
    private float climbRate = 5;
    private Vector3 swingDirection;
    private float swingSpeed = 90;

    Vector3 tensionDirection;
    Vector3 pendulumSideDirection;
    Vector3 tangentDirection;
    float tensionForce;
    public Vector3 forceDirection;
    private bool unhooking = false;


    // Start is called before the first frame update
    void Start()
    {
        isGrappled = false; 
        hookPoints = GameObject.FindGameObjectsWithTag("HookPoint");
        movementController = gameObject.GetComponent<CharacterController>();
        playerMovement = gameObject.GetComponent<dPlayerMovement>();
        pStats = gameObject.GetComponent<PlayerStats>();
    }

    // Update is called once per frame
    void Update()
    {
        //if (!IsLocalPlayer) { return; }

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) && pStats.HasGrapple) //If grapple button is hit
        {
            if (!isGrappled) //If we are not grappling
            {
                hookPointIndex = FindHookPoint(); //Find the nearest hook point within max distance
                if (hookPointIndex != -1) //If there is a hookpoint
                {
                    hookPoint = hookPoints[hookPointIndex]; //The point we are grappling from
                    ropeLength = Vector3.Distance(gameObject.transform.position, hookPoint.transform.position) + 0.5f;
                    isGrappled = true; //toggle grappling state
                    unhooking = false;
                }
            } 
            else //Else we are grappling
            {
                isGrappled = false; //toggle grappling state to release
            }
        }
    }

    private void FixedUpdate()
    {
        //if (!IsLocalPlayer) { return; }
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
                
                forceDirection = calculateForceDirection(1, playerMovement.g, hookPoint.transform.position);

            }
            movementController.Move(swingMoveController());
        }
        else if(!isGrappled && !unhooking){
            this.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            unhooking = true;
        }
        if(playerMovement.GetJumpPressed()) isGrappled = false;

        if(isGrappled == false) forceDirection = Vector3.zero;
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

    Vector3 calculateForceDirection(float mass, float g, Vector3 hPoint){
        tensionDirection = (hPoint - transform.position).normalized;

        float inclinationAngle = Vector3.Angle(transform.position - hPoint, -transform.up);

        tensionForce = mass * -g * Mathf.Cos(Mathf.Deg2Rad * inclinationAngle);

        Vector3 fDirection = tensionDirection * tensionForce;
        return fDirection;
    }
    

    Vector3 swingMoveController(){

        float inputVert = Input.GetAxis("Vertical");
        float inputHor = Input.GetAxis("Horizontal");

        if((!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))) inputVert = 0;

        if((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) inputHor = 0;

        //Movement Vector for the player to move in
        swingDirection = Vector3.Cross(tensionDirection, ((transform.right * -inputVert) + (transform.forward * inputHor))).normalized;
        //Actual swing  movement vector with speed applied
        Vector3 swingMovement = (swingDirection * Time.deltaTime * swingSpeed);

        return (swingMovement);
    }
}
