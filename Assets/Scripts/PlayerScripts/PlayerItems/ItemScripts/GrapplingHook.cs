using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;

public class GrapplingHook : NetworkBehaviour
{
    public float maxGrappleDistance = 15;

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
    private Vector3 swingDirection;
    float inclinationAngle;
    float theta = -1;

    private float maxSwingSpeed = 90;
    private float minSwingSpeed = 10;
    private float swingSpeed = 0;

    private float swingMom = 40;
    private Vector3 tensionMomDirection;
    private Vector3 hookPointRight;
    private Vector3 momDirection;
    private Vector3 curXZDir;
    private Vector3 oldXZDir;
    private float flip = -1; // flip variables for swing momentum
    private bool swingback = false; //swing the player back
    private float midpointMom;

    Vector3 tensionDirection;
    float tensionForce;
    public Vector3 forceDirection;


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

        if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) && pStats.HasGrapple) //If grapple button is hit
        {
            if (!isGrappled) //If we are not grappling
            {
                hookPointIndex = FindHookPoint(); //Find the nearest hook point within max distance
                if (hookPointIndex != -1) //If there is a hookpoint
                {
                    hookPoint = hookPoints[hookPointIndex]; //The point we are grappling from
                    ropeLength = Vector3.Distance(gameObject.transform.position, hookPoint.transform.position) + 0.5f;
                    oldXZDir = (new Vector3(hookPoint.transform.position.x,0,hookPoint.transform.position.z) - new Vector3(transform.position.x,0,transform.position.z)).normalized;
                    curXZDir = (new Vector3(hookPoint.transform.position.x,0,hookPoint.transform.position.z) - new Vector3(transform.position.x,0,transform.position.z)).normalized;
                    isGrappled = true; //toggle grappling state
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
        if (!IsLocalPlayer) { return; }
        
        if (isGrappled)
        {
            Debug.DrawRay(gameObject.transform.position, (hookPoint.transform.position - gameObject.transform.position)); //Visual of line

            //Extend Hook
            if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.JoystickButton4))
            {
                ropeLength += climbRate * Time.deltaTime;
                if (ropeLength > maxGrappleDistance)
                {
                    ropeLength = maxGrappleDistance;
                }
                //Debug.Log(ropeLength.ToString());
            }
            
            //Retract Hook
            if (Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.JoystickButton5))
            {
                ropeLength -= climbRate * Time.deltaTime;
                if (ropeLength < 5)
                {
                    ropeLength = 5;
                }
                //Debug.Log(ropeLength.ToString());
            }
            //Debug.Log(Vector3.Distance(gameObject.transform.position, hookPoint.transform.position));
            //Calculate tether force direction based on hookpoint
            if (Vector3.Distance(gameObject.transform.position, hookPoint.transform.position) >= ropeLength )
            {
                
                forceDirection = calculateForceDirection(1, playerMovement.g+.01f, hookPoint.transform.position);

            }

            //apply special swing movement when aerial
            if(!playerMovement.isGrounded){
                movementController.Move(swingMoveController());
                if(swingMom != 0){
                    movementController.Move(calculateMomentumDirection(playerMovement.g, hookPoint.transform.position));
                    swingMom -= .5f;
                }
            }
            if(swingMom<0) swingMom = 0;
        }
        else if(!isGrappled || playerMovement.isGrounded){
            flip = -1;
            swingback = true;
            swingMom = 50;
        }
        //WILL NEED ADJUSTMENT OR REMOVAL IN THE FUTURE
        //ungrapple on jump
        if(playerMovement.GetJumpPressed()) isGrappled = false;

        //Reset force direction after unhook
        if(isGrappled == false) forceDirection = Vector3.zero;
    }

    //Finds the nearest hook to the player
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

    //Calculate the tether direction vector and how much force that vector needs
    Vector3 calculateForceDirection(float mass, float g, Vector3 hPoint){

        //tension direction and angle calculation
        tensionDirection = (hPoint - transform.position).normalized;
        inclinationAngle = Vector3.Angle((transform.position - hPoint).normalized, -transform.up);
        theta = Mathf.Deg2Rad * inclinationAngle;
        if(theta<=.1) theta = 0;

        //How much force the tension needs
        tensionForce = mass * -g * Mathf.Cos(theta);

        //force direction calculation based on tension direction and force
        Vector3 fDirection = tensionDirection * tensionForce;
        return fDirection;
    }

    Vector3 calculateMomentumDirection(float g, Vector3 hPoint){

        tensionMomDirection = (hPoint - transform.position).normalized;
        hookPointRight = Vector3.Cross((new Vector3(hPoint.x,0,hPoint.z) - new Vector3(transform.position.x,0,transform.position.z)), transform.up).normalized;
        momDirection = flip * Vector3.Cross(hookPointRight, tensionMomDirection).normalized;
        
        if(oldXZDir != curXZDir && flip == -1){
            flip = 1;
            midpointMom = swingMom;
            swingback = false;
            Debug.Log("flip");
        }

        if(swingback == false && swingMom <= (midpointMom*(.75f))){
            swingback = true;
            flip = -1;
            oldXZDir = (new Vector3(hookPoint.transform.position.x,0,hookPoint.transform.position.z) - new Vector3(transform.position.x,0,transform.position.z)).normalized;
            Debug.Log("swingback");
        }

        curXZDir = (new Vector3(hPoint.x,0,hPoint.z) - new Vector3(transform.position.x,0,transform.position.z)).normalized;
        Debug.DrawRay(transform.position,  momDirection * Time.deltaTime* 100, Color.green);
        return (momDirection * Time.deltaTime * swingMom);
    }
    
    //Special movement for the player while they swing
    Vector3 swingMoveController(){

        //WASD input
        float inputVert = Input.GetAxis("Vertical");
        float inputHor = Input.GetAxis("Horizontal");

        //input is zero when nothing is pressed to prevent button easing values
        if((!Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))) inputVert = 0;
        if((!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))) inputHor = 0;

        //Swing direction based on player input
        swingDirection = Vector3.Cross(tensionDirection, ((transform.right * -inputVert) + (transform.forward * inputHor))).normalized;

        //NEED TO ADD SWINGSPEED EASING
        //Swing movement with swing speed added
        Vector3 swingMovement = (swingDirection * Time.deltaTime * maxSwingSpeed);
        

        return (swingMovement);
    }
}
