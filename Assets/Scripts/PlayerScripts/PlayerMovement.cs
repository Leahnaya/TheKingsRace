using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMovement : NetworkBehaviour
{

    //Scripts
    public PlayerStats pStats;

    //Variable Section
    /////
    //Speed Variables
    public Vector3 vel;

    private Vector3 moveZ;
    private Vector3 moveX;
    public Vector3 driftVel;

    //Player prefab
    private GameObject parentObj;

    //Character Moving
    private CharacterController moveController;

    //Jump value
    public int curJumpNum; // current Jumps used
    private bool jumpHeld; // Is jump being held
    private bool jumpPressed; // Has Jump been pressed
    float coyJumpTimer = 0.1f; // Default Coyote Jump time
    float curCoyJumpTimer; // current Coyote Jump time
    public float lowJumpMultiplier; // Short jump multiplier
    public float fallMultiplier; // High Jump Multiplier

    //Gravity values
    public float g = 0; // the y velocity affected by player Grav
    private float maxG = -100; //The max downwards y velocity or g the player can have

    //Glide Values
    bool tempSet = false;
    float tempTraction = 0.0f;

    //Impact physics
    private float mass = 5.0F; // defines the character mass
    private Vector3 impact = Vector3.zero;
    private float distToGround;

    //Wallrunning
    private WallRun wallRun;

    //Ground Check
    public bool isGrounded; //Better custom is grounded 
    public float groundCheckDistance = 0.05f; //how far away from the ground to not be considered grounded
    private float lastTimeJumped = 0f; //Last time the player jumped
    const float jumpGroundingPreventionTime = 0.2f; // delay in checking if we are grounded after a jump
    const float groundCheckDistanceInAir = 0.07f; //How close we have to get to ground to start checking for grounded again
    private Ray groundRay;
    private RaycastHit groundHit;

    //Camera Variables
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
    public bool isSliding = false;
    private float originalTraction;
    private RaycastHit ray;
    private Vector3 up;
    private bool qDown;
    private float tempCurVel;

    //Blink
    private Blink blink;

    //Grapple
    private GrapplingHook grapple;
    

    //Animation controller
    Animator animator;

    void Awake()
    {
        //Initialize Player Components
        moveController = GetComponent<CharacterController>(); // Character Controller
        rB = GetComponent<Rigidbody>(); //Rigid Body
        capCol = GetComponent<CapsuleCollider>(); // Capsule Collider
        capCol.enabled = true;
        parentObj = transform.parent.gameObject;
        animator = GetComponent<Animator>();

        //Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // PlayerStats
        wallRun = GetComponent<WallRun>(); //Wallrun
        blink = GetComponent<Blink>(); //Blink
        grapple = GetComponent<GrapplingHook>();

        //Get parents up direction
        up = GetComponentInParent<Transform>().up;
//
        //Coyote Timer Initialization
        curCoyJumpTimer = coyJumpTimer;
    }

    void Start()
    {
        distToGround = GetComponent<Collider>().bounds.extents.y;

        // Don't do movement unless this is the local player controlling it
        // Otherwise we let the server handle moving us

        if (!IsLocalPlayer) { return; }

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
        if (!IsLocalPlayer) { return; }

        //Controls for camera
        if(cam.enabled) Rotation();  
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

        //If Character controller is disabled use rigidbody calculations
        else{
            //if ragdoll timer is over disable ragdolling
            if (RagdollTimer() == 0){
                firstHit = false;
                DisableRagdoll();
            }   

            //Gravity without moveController
            g -= pStats.PlayerGrav * Time.deltaTime;
            rB.AddForce(new Vector3(0,g,0));
        }
        Respawn();
        
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
        Vector3 moveXZ = new Vector3(vel.x, 0, vel.z);
        driftVel = Vector3.Lerp(driftVel, moveXZ, pStats.Traction * Time.deltaTime);

        //Gravity and Jump calculations
        UpdateGravity();
        Jump();
        Vector3 moveY = new Vector3(0,g,0);

        //Slide Function
        Slide();


        //move animation 
        //if vel from input is greater than 0, start sprinting animation
        if (PlayerSpeed() > 0.1)
        {
            //Debug.Log(driftVel.magnitude);
            
            if(animator != null) animator.SetBool("isRunning", true);
        }
        //if low enough movement from player (this will be still at this value) stop animation
        else if (driftVel.magnitude < .05f)
        {
            driftVel = Vector3.zero;
            if(animator != null) animator.SetBool("isRunning", false);
        }
        //Move Player
        if(grapple.isGrappled && !isGrounded){
            moveController.Move(((moveY + grapple.forceDirection) * Time.deltaTime)); 
        } 
        else{
            moveController.Move(driftVel + (moveY * Time.deltaTime));
        }
        
        
    }



    //Calculates speed current player needs to be going
    public float PlayerSpeed()
    {
        WallCheck();
        //If nothing is pressed speed is 0
        if ((Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f) || isSliding ||(grapple.isGrappled && !isGrounded))
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

    private void WallCheck(){
        //IMPLEMENT A RAYCAST CHECK   

    }

    //Apply Impact for when force needs to be applied without ragdolling
    public void AddImpact(Vector3 dir, float force)
    {
        if (!IsLocalPlayer) { return; }

        //Normalize direction multiply by force and add it to the impact
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }



    //Jump Function
    private void Jump()
    {
        //If space/south gamepad button is pressed apply an upwards force to the player
        if (Input.GetAxis("Jump") != 0 && !jumpHeld && curJumpNum < pStats.JumpNum && !isSliding)
        {
            if(wallRun.IsWallRunning()){
                AddImpact((wallRun.GetWallJumpDirection()), pStats.JumpPow * 8f);
                g = pStats.JumpPow;
            }

            else{
                g = pStats.JumpPow;
            }

            curJumpNum++;
            jumpHeld = true;
            jumpPressed = true;
        }

        //Last time Jumped
        lastTimeJumped = Time.time;

        //If grounded no jumps have been used and coyote Timer is refreshed
        if(isGrounded && g == 0){
            curCoyJumpTimer = coyJumpTimer;
            curJumpNum = 0;
        } 
        //else start the coyote timer
        else curCoyJumpTimer -= Time.deltaTime;

        //if jump is being held coyote timer is zero
        if(jumpHeld) curCoyJumpTimer = 0;

        if(grapple.isGrappled && curJumpNum == pStats.JumpNum) curJumpNum = 0;

        //If space/south face gamepad button isn't being pressed then jump is false
        if (Input.GetAxis("Jump") == 0){
           jumpHeld = false;
        }

        if(g < 0){
            jumpPressed = false;
        }
    }

    //Get and update PlayerValues for other scripts
    //Get jumpHeld
    public bool GetJumpPressed(){
        return jumpHeld;
    }

    //Get player cam
    public Camera GetPlayerCamera()
    {
        return cam;
    }

    //Update Player Velocity
    public void AddPlayerVelocity(Vector3 additiveVelocity)
    {
        vel += additiveVelocity;
    }

    //Set Player Velocity
    public void SetPlayerVelocity(Vector3 newVelocity)
    {
        vel = newVelocity;
    }

    //Reduce player Jump
    public void decrementCurrentJumpNumber()
    {
        curJumpNum--;
    }



    //Camera and player rotation
    private void Rotation()
    {
        //If moveController is enabled allow Camera control
        if(moveController.enabled){
            //if input is received from Mouse X
            if (Input.GetAxis("Mouse X") != 0){
                transform.parent.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));
            }

            //if input is received from right analog stick (horizontal)
            else if(Input.GetAxis("HorizontalCam") != 0){
                transform.parent.Rotate(Vector3.up * sensitivity * Time.deltaTime * Input.GetAxis("HorizontalCam"));
            }

            //if input is if input is received from Mouse Y
            if (Input.GetAxis("Mouse Y") != 0)
            {
                camRotation.x -= Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
                camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);
                cam.transform.localEulerAngles = camRotation;
            }

            //if input is received from right analog stick (vertical)
            else if (Input.GetAxis("VerticalTurn") != 0){
                camRotation.x -= Input.GetAxis("VerticalTurn") * sensitivity * Time.deltaTime;
                camRotation.x = Mathf.Clamp(camRotation.x, minAngle, maxAngle);
                cam.transform.localEulerAngles = camRotation;
            }
        }
    }



    //REMOVE WHEN UNNECCESARY
    //Respawns player if they fall below a certain point
    private void Respawn()
    {
        if (transform.position.y < -5)
        {
            TeleportPlayer(new Vector3(0,100,0));
        }
    }



   //Gravity Function for adjusting y-vel due to wallrun/glide/etc
    private void UpdateGravity(){

        //Gliding
        if(pStats.HasGlider && g < 0 && Input.GetButton("Jump")){
            //Gravity with glider
            GravityCalculation(6);

            //Set temp values to put traction back to normal
            if(tempSet == false){
                tempTraction = pStats.Traction;
                pStats.Traction = 1.0f;
                tempSet = true;
            }
        }
        //if temporary values have been set restore them back to the normal values
        else if(pStats.HasGlider && g==0 && tempSet == true){
            pStats.Traction = tempTraction;
            tempSet = false;
        }

        //Wallrunning
        else if (pStats.HasWallrun) {
            //Run wall run script
            wallRun.WallRunRoutine();

            //if wallrunning apply different gravity
            if(wallRun.IsWallRunning()){
                GravityCalculation(4);
            }

            //Normal gravity if not wallrunning
            else{
                GravityCalculation(pStats.PlayerGrav);
            }
        }

        //Default Gravity
        else{

            //Normal gravity
            GravityCalculation(pStats.PlayerGrav);
        }
    }

    //Uses Given gravity to apply a downwards force while allowing coyote Jump and short hops
    private void GravityCalculation(float grav){
        //apply slight upwards force for jump smoothing when g < 0
        if(g < 0){
            g += grav * (fallMultiplier - 1) * Time.deltaTime;
        }

        //apply smaller upwards force if jump is released early when jumping creating a short jump
        else if (g > 0 && !Input.GetButton("Jump")){
            g += grav * (lowJumpMultiplier - 1) * Time.deltaTime;
        }

        //apply gravity if not grounded and coyote timer is less than 0
        if((isGrounded == false && curCoyJumpTimer <= 0) || grapple.isGrappled){
            g -= grav * Time.deltaTime;
        }
        //else don't apply gravity
        else{
            g = 0;
        }
        
        //Caps out the players downwards speed
        if(g < maxG){
            g = maxG;
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

        if (Physics.Raycast(groundRay, out groundHit, moveController.height + groundCheckDistance) && !jumpPressed ) //&& Time.time >= lastTimeJumped + jumpGroundingPreventionTime)  // only try to detect ground if it's been a short amount of time since last jump; otherwise we may snap to the ground instantly after we try jumping
        {
            // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
            if (Vector3.Dot(groundHit.normal, transform.up) > 0f)
            {
                isGrounded = true;
                // handle snapping to the ground
                if (groundHit.distance > moveController.skinWidth && !grapple.isGrappled)
                {
                    moveController.Move(Vector3.down * groundHit.distance);
                }
            }
        }
    }

    //Ragdoll Functions
    public void GetHit(Vector3 dir, float force){
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
        CancelMomentum();
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
        //if the q button or the east face button on gamepad is held down
        if (Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) {
            qDown = true;
            if (isSliding == false){
                originalTraction = pStats.Traction;
                this.gameObject.transform.eulerAngles = new Vector3(this.transform.eulerAngles.x - 90, this.transform.eulerAngles.y, this.transform.eulerAngles.z);
                isSliding = true;
                //if it can't find the animator (capsul prefab)
                if (GetComponent<Animator>() == null){
                    moveController.height = 1.0f;
                }
                //if the regular model
                else {
                    moveController.height *= .5f;
                }
                pStats.Traction = 0.01f;
          
            }
            tempCurVel = driftVel.magnitude * 50f;
            transform.Rotate(Vector3.forward * -sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));
            pStats.Traction += .004f;
        }
        else{
            qDown = false;
        }
        //NOTE: potentialy change this to only allow player back up if there is nothing above them
        if (qDown == false && isSliding == true) {
            //if nothing is above the object, stop sliding
            if (Physics.Raycast(this.gameObject.transform.position, up, out ray, 5f) == false)
            {
                this.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                isSliding = false;
                pStats.CurVel = tempCurVel;
                //if it can't find the animator (capsul prefab)
                if (GetComponent<Animator>() == null)
                {
                    moveController.height = 2.0f;
                }
                //if the regular model
                else
                {
                    moveController.height *= 2.0f;
                }
                pStats.Traction = originalTraction;
            }
            else{
                Debug.Log("Object above you");
                
            }
        }
    }

    public void CancelMomentum()
    {
        pStats.CurVel = 0;
        vel = Vector3.zero;
        moveX = Vector3.zero;
        moveZ = Vector3.zero;
        driftVel = Vector3.zero;
    }

    private IEnumerator RespawnTimer()
    {
        float duration = 2f;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        moveController.enabled = true;
    }

    public void TeleportPlayer(Vector3 position, Quaternion rotation = new Quaternion()){
        transform.position = position;
        transform.rotation = rotation;
    }
}
