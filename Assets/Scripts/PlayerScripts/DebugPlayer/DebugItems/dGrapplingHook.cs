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

    Vector3 tensionDirection;
    Vector3 pendulumSideDirection;
    Vector3 tangentDirection;
    float tensionForce;
    public Vector3 forceDirection;


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
                }
            } 
            else //Else we are grappling
            {
                //physics tear down?
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
                //Impact Based
                forceDirection = calculateForceDirection(1, playerMovement.g, hookPoint.transform.position);

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

    Vector3 calculateForceDirection(float mass, float g, Vector3 hPoint){
        tensionDirection = (hPoint - transform.position).normalized;
        Debug.Log(tensionDirection);

        float inclinationAngle = Vector3.Angle(transform.position - hPoint, -transform.up);


        tensionForce = mass * -g * Mathf.Cos(Mathf.Deg2Rad * inclinationAngle);
        Debug.Log(g);

        Vector3 fDirection = tensionDirection * tensionForce;
        Debug.Log(fDirection);
        return fDirection;
    }
    
}
