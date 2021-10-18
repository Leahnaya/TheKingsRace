using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : MonoBehaviour
{

    //Scripts
    public PlayerStats pStats;

    //Variable Section
    /////
    //Speed Variables
    private Vector3 moveZ;
    private Vector3 moveX;
    private Vector3 vel;
    private Vector3 driftVel;
    
    //Character Moving
    private CharacterController moveController;

    //Jump value
    private int curJumpNum;
    private bool jumpPressed;

    //Jump physics
    private float mass = 5.0F; // defines the character mass
    private Vector3 impact = Vector3.zero;
    private float distToGround;


    //Wallrunning
    private WallRun wallRun;

    //Ground Check
    public bool isGrounded { get; private set; } //Better custom is grounded 
    public float groundCheckDistance = 0.05f; //how far away from the ground to not be considered grounded
    private float lastTimeJumped = 0f; //Last time the player jumped
    const float jumpGroundingPreventionTime = 0.2f; // delay in checking if we are grounded after a jump
    const float groundCheckDistanceInAir = 0.07f; //How close we have to get to ground to start checking for grounded again
    public LayerMask groundCheckLayers = -1; //Physics layers checked to consider the player grounded


    //Camera Variables
    private LayerMask ignoreP;
    private Vector3 camRotation;
    private Camera cam;

    [Range(-45, -15)]
    public int minAngle = -30;
    [Range(30, 80)]
    public int maxAngle = 45;
    [Range(50, 500)]
    public int sensitivity = 200;

    //Blink Variables
    private LineRenderer beam;
    private Vector3 origin;
    private Vector3 endPoint;
    private Vector3 mousePos;
    private RaycastHit hit;
    /////

    void Awake(){
        //Initialize Components
        moveController = GetComponent<CharacterController>();
        pStats = GetComponent<PlayerStats>();
        Cursor.lockState = CursorLockMode.Locked;
        ignoreP = LayerMask.GetMask("Player");


        beam = gameObject.AddComponent<LineRenderer>();
        beam.startWidth = 0.2f;
        beam.endWidth = 0.2f;
        beam.enabled = false;

        //camera transform
        cam = Camera.main;

        //Wallrun
        wallRun = gameObject.GetComponent<WallRun>();
    }



    void Start(){
        distToGround = GetComponent<Collider>().bounds.extents.y;

    }

    // Update is called once per frame
    void FixedUpdate(){   
        //input controls for movement
        InputController();

        //Controls for camera
        Rotate();


        //if suffiecient impact magnitude is applied then move player
        if (impact.magnitude > 0.2F) moveController.Move(impact * Time.deltaTime);

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);

        //Checks if player should respawn
        Respawn();
        Blink();

    }



    //Reads inputs and moves player
    private void InputController(){
        //Check if player is grounded before each frame
        GroundCheck();
        //Keyboard inputs

        //Checks if movement keys have been pressed and calculates correct vector
        moveX = transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed();
        moveZ = transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * PlayerSpeed();
        
        //Adds vectors based on movement keys and other conditions to check what the
        //player vector should be under the circumstances
        vel = moveX + moveZ;

        //Gravity
        Gravity();

        driftVel = Vector3.Lerp(driftVel, vel, pStats.Traction*Time.deltaTime);
        //Jump Function
        Jump();

        moveController.Move(driftVel);
    } 



    //Calculates speed current player needs to be going
    public float PlayerSpeed(){
        //If nothing is pressed speed is 0
        if(Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f){
            pStats.CurVel = 0.0f;
            return pStats.CurVel; 
        }
        //If current speed is below min when pressed set to minimum speed
        else if(pStats.CurVel < pStats.MinVel){
            pStats.CurVel = pStats.MinVel;
            return pStats.MinVel;
        }
        // while the speed is below max speed slowly increase it
        else if((pStats.CurVel >= pStats.MinVel) && (pStats.CurVel < pStats.MaxVel)){
            pStats.CurVel += pStats.Acc;
            return pStats.CurVel;  
        }
        //If the players speed is above or equal to max speed set speed to max
        else{
            pStats.CurVel = pStats.MaxVel;
            return pStats.CurVel;
        }
    }



    //Applies impact in a direction with the given force
    public void AddImpact(Vector3 dir, float force){
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }



    //Jump Function
    private void Jump(){
        //If space is pressed apply an upwards force to the player
        if(Input.GetAxis("Jump") != 0 && !jumpPressed && curJumpNum+1 < pStats.JumpNum){
            AddImpact(transform.up, pStats.JumpPow);
            curJumpNum++;
            jumpPressed = true;
        }

        lastTimeJumped = Time.time;

        //NEEDS TO BE MASSIVELY CHANGE LIKELY USE RAYCAST TO CHECK IF ACTUALLY ON GROUND
        //CANNOT USE CHARACTERCONTROLLER.ISGROUNDED IT IS UNRELIABLE
        //If grounded no jumps have been used
        // if(IsGrounded()){
        //     curJumpNum = 0;
        // }

        //If space isn't being pressed then jump is false
        if(Input.GetAxis("Jump")==0) jumpPressed = false;
    }

    public bool GetJumpPressed() {
        return jumpPressed;
    }

    public Camera GetPlayerCamera() { 
        return cam; 
    }

    public void AddPlayerVelocity(Vector3 additiveVelocity) {
        vel += additiveVelocity;
    }

    public void SetPlayerVelocity(Vector3 newVelocity)
    {
        vel = newVelocity;
    }

    //Camera
    private void Rotate()
    {
        transform.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);

        cam.transform.localEulerAngles = camRotation;
    }



    //REMOVE WHEN UNNECCESARY
    //Respawns player if they fall below a certain point
    private void Respawn(){
        if(transform.position.y < -1){
            transform.position = new Vector3(1f, 1f, 1f);
        }
    }

    //Gravity Function for adjusting y-vel due to wallrun/glide/etc
    private void Gravity(){
        //Normal Gravity
        vel.y -= pStats.PlayerGrav * Time.deltaTime; 
        //Wallrunning
        if (pStats.HasWallrun) { wallRun.WallRunRoutine(); } //adjusted later if we are wallrunning
        //If gliding 
            //Go down slowly
    }

    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (moveController.skinWidth + groundCheckDistance) : groundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        Vector3 groundNormal = Vector3.up;

        // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        if (Time.time >= lastTimeJumped + jumpGroundingPreventionTime)
        {
            // if we're grounded, collect info about the ground normal with a downward capsule cast representing our character capsule
            if (Physics.CapsuleCast(GetCapsuleBottomHemisphere(), GetCapsuleTopHemisphere(moveController.height), moveController.radius, Vector3.down, out RaycastHit hit, chosenGroundCheckDistance, groundCheckLayers, QueryTriggerInteraction.Ignore))
            {
                // storing the upward direction for the surface found
                groundNormal = hit.normal;

                // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
                // and if the slope angle is lower than the character controller's limit
                if (Vector3.Dot(hit.normal, transform.up) > 0f && IsNormalUnderSlopeLimit(groundNormal))
                {
                    isGrounded = true;

                    // handle snapping to the ground
                    if (hit.distance > moveController.skinWidth)
                    {
                        moveController.Move(Vector3.down * hit.distance);
                    }
                }
            }
        }
    }

    private bool IsNormalUnderSlopeLimit(Vector3 normal){  // Returns true if the slope angle represented by the given normal is under the slope angle limit of the character controller
        return Vector3.Angle(transform.up, normal) <= moveController.slopeLimit;
    }

    private Vector3 GetCapsuleBottomHemisphere(){  // Gets the center point of the bottom hemisphere of the character controller capsule    
        return transform.position + (transform.up * moveController.radius);
    }

    private Vector3 GetCapsuleTopHemisphere(float atHeight){  // Gets the center point of the top hemisphere of the character controller capsule    

        return transform.position + (transform.up * (atHeight - moveController.radius));
    }

    //ADJUST SO DISTANCE IS DETERMINED BY SCROLL WHEEL
    //blinks the player forwards
    private void Blink(){
        if (Input.GetMouseButton(1)){
            // Finding the origin and end point of laser.
            origin = transform.position + transform.forward * transform.lossyScale.z;

            // Finding mouse pos in 3D space.
            mousePos = Input.mousePosition;
            mousePos.z = 20f;
            endPoint = cam.ScreenToWorldPoint(mousePos);

            // Find direction of beam.
            Vector3 dir = endPoint - origin;
            dir.Normalize();

            // Are we hitting any colliders?
            if (Physics.Raycast(origin, dir, out hit, 20f)){
                // If yes, then set endpoint to hit-point.
                endPoint = hit.point;
            }

            // Set end point of laser.
            beam.SetPosition(0, origin);
            beam.SetPosition(1, endPoint);
            // Draw the laser!
            beam.enabled = true;
            /*Ray ray = camera.ScreenPointToRay(Input.mousePosition);
            RaycastHit raycastHit;

            if (Physics.Raycast(ray, out raycastHit, 5.0f)){
            LineRenderer.SetPosition(1, raycastHit.point);
            }*/

        }

        else if(!Input.GetMouseButton(1) && beam.enabled == true){
            beam.enabled = false;
            //if teleporting due to hit to object, bump them a bit outside normal
            if(hit.point != null) {
                transform.position = endPoint + hit.normal * 1.25f;

            }
            //if teleporting in the air or something, just spawn at endpoint
            else{

                transform.position = endPoint;
            }
            //reenable character controller
        }
    }

    
}
