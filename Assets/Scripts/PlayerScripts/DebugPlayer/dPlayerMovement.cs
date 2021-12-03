using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

public class dPlayerMovement : NetworkBehaviour
{

    //Scripts
    public PlayerStats pStats;

    //Variable Section
    /////
    //Speed Variables
    public Vector3 vel;

    private Vector3 moveZ;
    private Vector3 moveX;
    private Vector3 driftVel;

    //Player prefab
    private GameObject parentObj;

    //Character Moving
    private CharacterController moveController;

    //Jump value
    public int curJumpNum;
    private bool jumpPressed;
    bool tempSet = false;
    float tempTraction = 0.0f;

    //Jump physics
    private float mass = 5.0F; // defines the character mass
    private Vector3 impact = Vector3.zero;
    private float distToGround;

    //Wallrunning
    private dWallRun wallRun;

    //Ground Check
    public bool isGrounded; //Better custom is grounded 
    public float groundCheckDistance = 0.05f; //how far away from the ground to not be considered grounded
    private float lastTimeJumped = 0f; //Last time the player jumped
    const float jumpGroundingPreventionTime = 0.2f; // delay in checking if we are grounded after a jump
    const float groundCheckDistanceInAir = 0.07f; //How close we have to get to ground to start checking for grounded again
    private Ray groundRay;
    private RaycastHit groundHit;

    //Camera Variables
    private LayerMask ignoreP;
    private Vector3 camRotation;
    public Camera cam;

    [Range(-45, -15)]
    public int minAngle = -30;
    [Range(30, 80)]
    public int maxAngle = 45;
    [Range(50, 500)]
    public int sensitivity = 200;

    //Ragdoll variables
    private Vector3 hit;
    private Rigidbody rB;
    private CapsuleCollider capCol;
    private bool firstHit = false;
    //private bool heldDown = false; //Variable for testing Ragdoll reenable if needed
    private bool beginRagTimer = false;
    private float ragTime; 
    private Vector3 prevRot;
    private Vector3 hitForce;

    //Slide Variables
    private bool isSliding = false;
    private float originalTraction;
    private RaycastHit ray;
    private Vector3 up;
    private bool qDown;

    //Blink
    private dBlink blink;

    void Awake()
    {
        //Initialize Components
        moveController = GetComponent<CharacterController>();
        rB = GetComponent<Rigidbody>();
        capCol = GetComponent<CapsuleCollider>();
        pStats = GetComponent<PlayerStats>();
        parentObj = transform.parent.gameObject;

        capCol.enabled = false;
        //Wallrun
        wallRun = gameObject.GetComponent<dWallRun>();

        blink = gameObject.GetComponent<dBlink>();

        up = this.gameObject.GetComponentInParent<Transform>().up;
    }

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;

        // Don't do movement unless this is the local player controlling it
        // Otherwise we let the server handle moving us

        //if (!IsLocalPlayer) { return; }

        // Don't lock the cursor multiple times if this isn't the local player
        // Also don't want to lock the cursor for the king
        // That is why this is after the LocalPlayer check
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Don't do movement unless this is the local player controlling it
        // Otherwise we let the server handle moving us
        //if (!IsLocalPlayer) { return; }

        
        //Controls for camera
        if(cam.enabled){
          Rotation();  
        }
        else Debug.Log("Cam Disabled");
        
        
        //Allow Movement when moveController is enabled
        if(moveController.enabled == true){

            //input controls for movement
            InputController();

           //if suffiecient impact magnitude is applied then move player
            if (impact.magnitude > 0.2F) moveController.Move(impact * Time.deltaTime);

            // consumes the impact energy each cycle:
            impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
        }
        else{

            if (RagdollTimer() == 0){ 
        
                firstHit = false;
                DisableRagdoll();
            }   

            //Gravity without moveController
            vel.y -= pStats.PlayerGrav * Time.deltaTime;
            rB.AddForce(new Vector3(0,vel.y,0));
            
            //Debug.LogWarning("MoveController is either Disabled or wasn't retrieved correctly");
        }

        //TEMP FOR TESTING RAGDOLL
        //Right Click to ragdoll the player
        // if (Input.GetMouseButton(1) && heldDown == false){
        //     getHit(new Vector3(vel.x, 0, vel.z), 30);
        //     heldDown = true;
        // }
        // if(!Input.GetMouseButton(1)){
        //     heldDown = false;
        // }

        if(pStats.HasBlink == true){
            blink.BlinkMove();
        }

        //TEMP FOR TESTING
        //Checks if player should respawn
        //Respawn();
        
    }



    //Reads inputs and moves player
    private void InputController()
    {
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

        driftVel = Vector3.Lerp(driftVel, vel, pStats.Traction * Time.deltaTime);

        //Moving outside basic wasd
        //Jump Function
        Jump();
        //Slide Function
        Slide();
        
        //Move Player
        moveController.Move(driftVel);
    }



    //Calculates speed current player needs to be going
    public float PlayerSpeed()
    {
        //If nothing is pressed speed is 0
        if ((Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f) || isSliding)
        {
            pStats.CurVel = 0.0f;
            return pStats.CurVel;
        }
        //If current speed is below min when pressed set to minimum speed
        else if (pStats.CurVel < pStats.MinVel)
        {
            pStats.CurVel = pStats.MinVel;
            return pStats.MinVel;
        }
        // while the speed is below max speed slowly increase it
        else if ((pStats.CurVel >= pStats.MinVel) && (pStats.CurVel < pStats.MaxVel))
        {
            pStats.CurVel += pStats.Acc;
            return pStats.CurVel;
        }
        //If the players speed is above or equal to max speed set speed to max
        else
        {
            pStats.CurVel = pStats.MaxVel;
            return pStats.CurVel;
        }
    }



    //Apply Impact for when force needs to be applied without ragdolling
    public void AddImpact(Vector3 dir, float force)
    {
        
        //if (!IsLocalPlayer) { return; }

        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }



    //Jump Function
    private void Jump()
    {
        //If space is pressed apply an upwards force to the player
        if (Input.GetAxis("Jump") != 0 && !jumpPressed && curJumpNum + 1 < pStats.JumpNum && !isSliding)
        {
            if(wallRun.IsWallRunning()){
                AddImpact((wallRun.GetWallJumpDirection()), pStats.JumpPow * 1.3f);
                AddImpact(transform.up, pStats.JumpPow);
            }
            else{
                AddImpact(transform.up, pStats.JumpPow);
            }

            curJumpNum++;
            jumpPressed = true;
        }

        lastTimeJumped = Time.time;

        //If grounded no jumps have been used
        if(isGrounded){
             curJumpNum = 0;
         }

        //If space isn't being pressed then jump is false
        if (Input.GetAxis("Jump") == 0) jumpPressed = false;
    }



    //PlayerScript
    public bool GetJumpPressed(){
        return jumpPressed;
    }

    public Camera GetPlayerCamera()
    {
        return cam;
    }

    public void AddPlayerVelocity(Vector3 additiveVelocity)
    {
        vel += additiveVelocity;
    }

    public void SetPlayerVelocity(Vector3 newVelocity)
    {
        vel = newVelocity;
    }



    //Camera and Player Rotation
    private void Rotation()
    {
        Vector3 lastCamPos = new Vector3(0,0,0);
        Vector3 rotOffset = transform.localEulerAngles; 
        if(moveController.enabled){
        transform.parent.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));


        camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);

        cam.transform.localEulerAngles = camRotation;
        }
    }



    //REMOVE WHEN UNNECCESARY
    //Respawns player if they fall below a certain point
    private void Respawn()
    {
        if (transform.position.y < -1)
        {
            transform.position = new Vector3(1f, 3f, 1f);
        }
    }



    //Gravity Function for adjusting y-vel due to wallrun/glide/etc
    private void Gravity(){

        //Gliding
        if(jumpPressed && pStats.HasGlider){
            
            vel.y -= (pStats.PlayerGrav-18) * Time.deltaTime;
            if(tempSet == false){
                tempTraction = pStats.Traction;
                pStats.Traction = 1.0f;
                tempSet = true;
            }
        }
        else if(!jumpPressed && pStats.HasGlider){

            if(tempSet == true){
               pStats.Traction = tempTraction;
               tempSet = false; 
            }

            //Normal Gravity
            vel.y -= pStats.PlayerGrav * Time.deltaTime;
        }

        //Wallrunning
        else if (pStats.HasWallrun) { wallRun.WallRunRoutine(); } //adjusted later if we are wallrunning

        else{
            vel.y -= pStats.PlayerGrav * Time.deltaTime;
        }
    }


    //Checks if player is grounded
    void GroundCheck()
    {
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (moveController.skinWidth + groundCheckDistance) : groundCheckDistanceInAir;
        // reset values before the ground check
        isGrounded = false;
        groundRay = new Ray(moveController.transform.position, Vector3.down);
        if (Physics.Raycast(groundRay, out groundHit, moveController.height + groundCheckDistance)) //&& Time.time >= lastTimeJumped + jumpGroundingPreventionTime)  // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        {
            // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
            if (Vector3.Dot(groundHit.normal, transform.up) > 0f)
            {
                isGrounded = true;
                // handle snapping to the ground
                if (groundHit.distance > moveController.skinWidth)
                {
                    moveController.Move(Vector3.down * groundHit.distance);
                }
            }
        }
    }

    //Ragdoll Functions
    private void GetHit(Vector3 dir, float force){
        if(firstHit == false){
            EnableRagdoll();
            dir.Normalize();
            rB.AddForce(dir * force, ForceMode.Impulse);
            firstHit = true;
        }
    }
    private void EnableRagdoll(){
        ragTime = pStats.RecovTime;
        prevRot = transform.localEulerAngles;
        capCol.enabled = true;
        moveController.enabled = false;
        rB.isKinematic = false;
        rB.detectCollisions = true;
    }

    private void DisableRagdoll(){
        capCol.enabled = false;
        moveController.enabled = true;
        rB.isKinematic = true;
        rB.detectCollisions = false;
        transform.localEulerAngles = prevRot;
    }

    //When to begin the ragdoll timer
    private float RagdollTimer(){
        if(beginRagTimer == false){
            beginRagTimer = Physics.Raycast(transform.position, -Vector3.up, distToGround + 1f);
        }

        else if(ragTime <= 0){
            ragTime = 0;
            beginRagTimer = false;
        }

        if(beginRagTimer == true){
            ragTime -= Time.deltaTime;
        }

        return ragTime;
    }
    
    //Slide Function
    private void Slide(){
        if (Input.GetKey(KeyCode.Q)){
            qDown = true;
            if (isSliding == false){
                originalTraction = pStats.Traction;
                this.gameObject.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x - 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
                isSliding = true;
                moveController.height = 1.0f;
                pStats.Traction = 0.01f;
          
            }
            pStats.Traction += .004f;
        }
        else{
            qDown = false;
        }
        //NOTE: potentialy change this to only allow player back up if there is nothing above them
        if (qDown == false && isSliding == true) {
            //if nothing is above the object, stop slidding
            if (Physics.Raycast(this.gameObject.transform.position, up, out ray, 5f) == false)
            {
                this.gameObject.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x + 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
                isSliding = false;
                moveController.height = 2.0f;
                pStats.Traction = originalTraction;

            }
            else{
                Debug.Log("Object above you");

            }
        }
    }

}
