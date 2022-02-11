using System.Collections;
using System.Collections.Generic;
using MLAPI;
using UnityEngine;
using UnityEngine.UI;

public class dPlayerMovement : NetworkBehaviour
{
    ////Objects Sections
    private GameObject parentObj; // Parent object
    public Camera cam; // Camera object
    ////

    ////Components Section
    private CharacterController moveController; // Character Controller
    private Rigidbody rB; // Players Rigidbody
    private CapsuleCollider capCol; // Players Capsule Collider
    private Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    private dGrapplingHook grapple; // Grappling Hook
    private dNitro nitro; // Nitro
    private dWallRun wallRun; // Wallrun
    ////

    ////Player Variables Section
    //Speed Variables
    public Vector3 vel; // moveZ + moveX
    private Vector3 moveZ; // Local Horizontal Vector
    private Vector3 moveX; // Local Vertical Vector
    public Vector3 driftVel; // Lerped Movement Vector

    //Jump Variables
    public int curJumpNum; // current Jumps Used
    public bool jumpHeld; // Jump is Held
    private bool jumpPressed; // Jamp was pressed
    float coyJumpTimer = 0.1f; // Default Coyote Jump time
    float curCoyJumpTimer = 0.1f; // current Coyote Jump time
    public float lowJumpMultiplier; // Short jump multiplier
    public float fallMultiplier; // High Jump Multiplier

    //Gravity Variables
    public float g = 0; // player downwards velocity
    private float maxG = -100; // max downwards velocity

    //Glide Variables
    private bool tempSetTraction = false; // has the Traction been temporarily set
    private float tempTraction = 0.0f; // temporary traction

    //Impact Variables
    private float mass = 5.0F; // mass variable for Impact
    private Vector3 impact = Vector3.zero; // Impact Vector
    private float distToGround; // distance to ground

    //Ground Check
    public bool isGrounded; // is player grounded
    public float groundCheckDistance = 0.05f; // offset distance to check ground
    private float lastTimeJumped = 0f; // Last time the player jumped
    private const float jumpGroundingPreventionTime = 0.2f; // delay so player doesn't get snapped to ground while jumping
    private const float groundCheckDistanceInAir = 0.07f; // How close we have to get to ground to start checking for grounded again
    private Ray groundRay; // ground ray
    private RaycastHit groundHit; // ground raycast

    //Camera Variables
    private Vector3 camRotation; // cameras camera rotation vector
    [Range(-45, -15)]
    public int minAngle = -30; // minimum downwards cam angle
    [Range(30, 80)]
    public int maxAngle = 45; // Max upwards cam angle
    [Range(50, 500)]
    public int sensitivity = 200; // Camera sensitivity

    //Ragdoll variables
    private bool firstHit = false; // has first contact happened
    private bool beginRagTimer = false; // beging ragdoll timer bool
    private float ragTime; // ragdoll timer
    private Vector3 prevRot; // Save last rotation before hit

    //Slide Variables
    public bool isSliding = false; // If player is sliding /////////// Maybe unnecessary once state machine is implemented
    private float originalTraction; // Original Traction
    private RaycastHit slideRay; // slide raycast
    private Vector3 slideUp; // Slide upwards direction
    private bool qDown; // is q being pressed
    private float tempSlideCurVel; // temp vel while sliding so they don't lose speed
    ////

    void Awake(){
        ////Initialize Player Components
        moveController = GetComponent<CharacterController>(); // set Character Controller
        rB = GetComponent<Rigidbody>(); //set Rigid Body
        capCol = GetComponent<CapsuleCollider>(); // set Capsule Collider
        capCol.enabled = true;
        parentObj = transform.parent.gameObject; // set parent object
        animator = GetComponent<Animator>(); // set animator
        ////

        ////Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // set PlayerStats
        wallRun = GetComponent<dWallRun>(); // set Wallrun
        grapple = GetComponent<dGrapplingHook>(); // set grapplingHook
        nitro = GetComponent<dNitro>(); // set Nitro
        ////
    }

    void Start(){
        ////Initialize important starting variables
        distToGround = GetComponent<Collider>().bounds.extents.y; // set players distance to ground
        slideUp = GetComponentInParent<Transform>().up; // get parents up direction
        ////

        // Don't do movement unless this is the local player controlling it
        // Otherwise we let the server handle moving us

        //if (!IsLocalPlayer) { return; }

        // Don't lock the cursor multiple times if this isn't the local player
        // Also don't want to lock the cursor for the king
        // That is why this is after the LocalPlayer check

        Cursor.lockState = CursorLockMode.Locked; // Lock cursor on start if you are the local player
    }

    // Update is called once per frame
    void FixedUpdate(){
        // Don't do movement unless this is the local player controlling it
        // Otherwise we let the server handle moving us
        //if (!IsLocalPlayer) { return; }

        //Controls for camera
        if(cam.enabled) Rotation();  
        else Debug.Log("Cam Disabled");
        
        //Allow Movement when moveController is enabled
        if(moveController.enabled == true){
            //input controls for movement
            InputController();
            
            //Dissipates Impact 
            DissipateImpact();
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

        /////ONLY FOR DEBUG PURPOSES REMOVE WHEN UNNECESSARY
        if (transform.position.y < -5)
        {
            TeleportPlayer(new Vector3(0,100,0));
        }
        
    }

    ////Input Related Functions
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
            driftVel = Vector3.zero;
            moveController.Move(((moveY + grapple.forceDirection) * Time.deltaTime)); 
        } 
        else{
            moveController.Move(driftVel + (moveY * Time.deltaTime));
        }
        
        
    }

    //Calculates speed current player needs to be going
    public float PlayerSpeed(){
        WallCheck();
        //If nothing is pressed speed is 0
        if ((Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f) || isSliding || (grapple.isGrappled && !isGrounded))
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
        else if (pStats.CurVel >= pStats.MaxVel && nitro.isNitroing == false)
        {
            pStats.CurVel = pStats.MaxVel;
            return pStats.CurVel;

        }
        else if(nitro.isNitroing){
            return pStats.CurVel;
        }
        else{
            Debug.Log("Something has gone wrong with the PlayerSpeed()");
            return -1;
        }
    }

    //Need to implement to check if player has hit a wall in the direction they are moving and they should lose speed
    private void WallCheck(){
        //IMPLEMENT A RAYCAST CHECK   

    }

    //Jump Function
    private void Jump(){
        //If space/south gamepad button is pressed apply an upwards force to the player
        if (Input.GetAxis("Jump") != 0 && !jumpHeld && curJumpNum < pStats.JumpNum && !isSliding)
        {
            if(wallRun.IsWallRunning()){
                AddImpact((wallRun.GetWallJumpDirection()), pStats.JumpPow * 8.5f);
                g = pStats.JumpPow;
                curJumpNum = 0;
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

    //Camera and player rotation
    private void Rotation(){
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

    //Gravity Function for adjusting y-vel due to wallrun/glide/etc
    private void UpdateGravity(){

        //Gliding
        if(pStats.HasGlider && g < 0 && Input.GetButton("Jump") && !isSliding){
            //Gravity with glider
            GravityCalculation(8);

            //Set temp values to put traction back to normal
            if(tempSetTraction == false){
                tempTraction = pStats.Traction;
                pStats.Traction = 1.0f;
                tempSetTraction = true;
            }
        }
        //if temporary values have been set restore them back to the normal values
        else if(pStats.HasGlider && g==0 && tempSetTraction == true){
            pStats.Traction = tempTraction;
            tempSetTraction = false;
        }

        //Wallrunning
        else if (pStats.HasWallrun) {
            //Run wall run script
            wallRun.WallRunRoutine();

            //if wallrunning apply different gravity
            if(wallRun.IsWallRunning()){
                if(wallRun.firstAttach){
                    g = 0;
                    wallRun.firstAttach = false;
                }
                GravityCalculation(2);
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

    //Slide Function
    private void Slide(){
        //if the q button or the east face button on gamepad is held down
        if ((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q))) {
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
            tempSlideCurVel = driftVel.magnitude * 50f;
            transform.Rotate(Vector3.forward * -sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));
            pStats.Traction += .004f;
        }
        else{
            qDown = false;
        }
        //NOTE: potentialy change this to only allow player back up if there is nothing above them
        if (qDown == false && isSliding == true) {
            //if nothing is above the object, stop sliding
            if (Physics.Raycast(this.gameObject.transform.position, slideUp, out slideRay, 5f) == false)
            {
                this.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
                isSliding = false;
                pStats.CurVel = tempSlideCurVel;
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
    ////

    ////Physics Calculation Unrelated to inputs
    //Apply Impact for when force needs to be applied without ragdolling
    public void AddImpact(Vector3 dir, float force){
        //if (!IsLocalPlayer) { return; }

        //Normalize direction multiply by force and add it to the impact
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    //Dissipates Impact Force
    private void DissipateImpact(){
        //if suffiecient impact magnitude is applied then move player
        if (impact.magnitude > 0.2F) moveController.Move(impact * Time.deltaTime);

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
    }

    //Clear all stored movement
    public void CancelMomentum(){
        pStats.CurVel = 0;
        vel = Vector3.zero;
        moveX = Vector3.zero;
        moveZ = Vector3.zero;
        driftVel = Vector3.zero;
    }

    //Checks if player is grounded
    void GroundCheck(){
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
    ////

    ////Ragdoll Functions
    //Player Gets Hit
    public void GetHit(Vector3 dir, float force){
        //if (!IsLocalPlayer) { return; }
        if(firstHit == false){
            EnableRagdoll();
            dir.Normalize();
            rB.AddForce(dir * force, ForceMode.Impulse);
            firstHit = true;
        }
    }

    //Enable Ragdoll and update all related variables
    private void EnableRagdoll(){
        ragTime = pStats.RecovTime;
        prevRot = transform.localEulerAngles;
        capCol.enabled = true;
        moveController.enabled = false;
        rB.isKinematic = false;
        rB.detectCollisions = true;
    }

    //Disable Ragdoll and revert all ragdoll variables
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
    ////

    //// Respawn and Relocation Functions
    //Respawn timer
    private IEnumerator RespawnTimer(){
        float duration = 2f;
        float normalizedTime = 0;
        while (normalizedTime <= 1f)
        {
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        moveController.enabled = true;
    }

    //Teleports player to new location
    public void TeleportPlayer(Vector3 position, Quaternion rotation = new Quaternion()){
        transform.position = position;
        transform.rotation = rotation;
    }
    ////
}
