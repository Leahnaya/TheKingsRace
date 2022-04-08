using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using MLAPI;
using UnityEngine.Rendering;

//Look into state set

public class AerialStateManager : NetworkBehaviour
{
    ////Player States
    public AerialBaseState currentState;
    public AerialBaseState previousState;

    //Aerial States
    public AerialFallingState FallingState = new AerialFallingState();
    public AerialGlidingState GlidingState = new AerialGlidingState();
    public AerialGroundedState GroundedState = new AerialGroundedState();
    public AerialJumpingState JumpingState = new AerialJumpingState();

    //Wallrunning States
    public AerialWallRunState WallRunState = new AerialWallRunState();
    public AerialWallIdleState WallIdleState = new AerialWallIdleState();

    //Grappling States
    public AerialGrappleAirState GrappleAirState = new AerialGrappleAirState();
    ////

    ////Objects Sections
    GameObject parentObj; // Parent object
    public Camera cam;
    ////

    ////Components Section
    public CharacterController moveController; // Character Controller
    Rigidbody rB; // Players Rigidbody
    public Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public MoveStateManager mSM;
    //// AnimatorManagerScript
    private AnimationManager animationManager;
    ////

    ////Variables Section
    //Jump Variables
    public int curJumpNum; // current Jumps Used
    public bool jumpHeld; // Jump is Held
    public bool canJump = true; // can the player jump
    bool jumpPressed; // Jamp was pressed
    public float coyJumpTimer = 0.1f; // Default Coyote Jump time
    public float curCoyJumpTimer = 0.1f; // current Coyote Jump time
    public float lowJumpMultiplier; // Short jump multiplier
    public float fallMultiplier; // High Jump Multiplier

    //Gravity Variables//
    float maxG = -100; // max downwards velocity

    //Ground Check
    public bool isGrounded; // is player grounded
    public float groundCheckDistance = 0.05f; // offset distance to check ground
    public float groundSlantDistance = 0.1f;
    const float jumpGroundingPreventionTime = 0.2f; // delay so player doesn't get snapped to ground while jumping
    const float groundCheckDistanceInAir = 0.07f; // How close we have to get to ground to start checking for grounded again
    Ray groundRay; // ground ray
    RaycastHit groundHit; // ground raycast
    
    //Wallrun
    float wallMaxDistance = 3f; // distance to wall that player can attach from
    float minimumHeight = .1f; // minimum height player has to be
    [Range(0.0f, 1.0f)]
    float normalizedAngleThreshold = 0.1f; // angle player can attach to wall from
    float jumpDuration = .02f; // jump duration
    float wallBouncing = 3; // wall bouncing
    Vector3[] directions; // cardinal direction to attach to wall
    RaycastHit[] hits; // where we hit wall
    public bool isWallRunning = false; // if player is wallrunning
    Vector3 lastWallPosition; // last wall position
    Vector3 lastWallNormal; // last wall normal
    float elapsedTimeSinceJump = 0; // time since jump
    float elapsedTimeSinceWallAttach = 0; // time since attached to wall
    float elapsedTimeSinceWallDetatch = 0; // time since detached
    bool jumping; // is player jumping

    //Impact Variables
    float mass = 5.0F; // mass variable for Impact
    Vector3 impact = Vector3.zero; // Impact Vector

    //Grapple Variables
    float maxGrabDistance = 25;// Max Distance can cast grapple
    public GameObject hookPoint; // Actual Hook points
    public GameObject[] hookPoints; // Hook point list
    public int hookPointIndex; // Hook point Index
    public float distance; // distance of hookpoints
    public bool release = false; // has player ungrappled
    public Vector3 postForceDirection; // force direction vector
    public float currentForcePower = 0;
    ////

    void Awake(){
        
        ////Initialize Player Components
        moveController = GetComponent<CharacterController>(); // set Character Controller
        rB = GetComponent<Rigidbody>(); //set Rigid Body
        parentObj = transform.parent.gameObject; // set parent object
        animator = GetComponent<Animator>(); // set animator
        ////

        ////Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // set PlayerStats
        mSM = GetComponent<MoveStateManager>(); // set move state manager
        animationManager = GetComponent<AnimationManager>();
        ////
    }

    void Start(){
        //players starting state
        currentState = GroundedState;
        previousState = GroundedState;
        currentState.EnterState(this, previousState);

        ////Initialize Variables
        //Wallrun Variables
        directions = new Vector3[]{ 
            Vector3.right, 
            Vector3.right + Vector3.forward,
            Vector3.forward, 
            Vector3.left + Vector3.forward, 
            Vector3.left
        };

        //Grapple Variables
        hookPoints = GameObject.FindGameObjectsWithTag("HookPoint");
        ////
    }

    void Update(){
        if(currentState == null){
            //players starting state
            currentState = GroundedState;
            previousState = GroundedState;
            currentState.EnterState(this, previousState);

            ////Initialize Variables
            //Wallrun Variables
            directions = new Vector3[]{ 
                Vector3.right, 
                Vector3.right + Vector3.forward,
                Vector3.forward, 
                Vector3.left + Vector3.forward, 
                Vector3.left
            };

            //Grapple Variables
            hookPoints = GameObject.FindGameObjectsWithTag("HookPoint");
            ////
        }

        if (!IsLocalPlayer) { return; }
        
        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        if (!IsLocalPlayer) { return; }

        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);

        //Some functions that need to be active if move controller is active
        if(moveController.enabled){
            
            //Allows player to jump
            Jump();

            //checks ground
            GroundCheck();

            //Applies downwards movement
            DownwardMovement();

            //if player has wallrun then do wall run routine 
            if(pStats.HasWallrun){
                WallRunRoutine();
            }

            //Dissipates Impact 
            DissipateImpact();
        }
        
        else{

            //Gravity without moveController
            pStats.GravVel -= pStats.PlayerGrav * Time.deltaTime;
            rB.AddForce(new Vector3(0,pStats.GravVel,0));
        }
        

        
    }

    public void SwitchState(AerialBaseState state){
        if (!IsLocalPlayer) { return; }

        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        animationManager.updateCurrentPriority();
        currentState.EnterState(this, previousState);
    }


    ////Jump & Gravity Calculations
    //Uses Given gravity to apply a downwards force while allowing coyote Jump and short hops
    public void GravityCalculation(float grav){
        if(moveController.enabled){

            //apply slight upwards force for jump smoothing when g < 0
            if(pStats.GravVel < 0){
                pStats.GravVel += grav * (fallMultiplier - 1) * Time.deltaTime;
            }

            //apply smaller upwards force if jump is released early when jumping creating a short jump
            else if (pStats.GravVel > 0 && !Input.GetButton("Jump")){
                pStats.GravVel += grav * (lowJumpMultiplier - 1) * Time.deltaTime;
            }

            //apply gravity if not grounded and coyote timer is less than 0
            if((isGrounded == false && curCoyJumpTimer <= 0) || currentState == GrappleAirState){
                pStats.GravVel -= grav * Time.deltaTime;
            }
            //else don't apply gravity
            else{
                pStats.GravVel = 0;
            }
            
            //Caps out the players downwards speed
            if(pStats.GravVel < maxG){
                pStats.GravVel = maxG;
            }
        }
    }
        
    //Checks if player is grounded
    void GroundCheck(){
        // Make sure that the ground check distance while already in air is very small, to prevent suddenly snapping to ground
        float chosenGroundCheckDistance = isGrounded ? (moveController.skinWidth + groundCheckDistance) : groundCheckDistanceInAir;

        // reset values before the ground check
        isGrounded = false;
        groundRay = new Ray(moveController.transform.position, Vector3.down);

        if (Physics.Raycast(groundRay, out groundHit, moveController.height + groundCheckDistance) && !jumpPressed)
        {
            // Only consider this a valid ground hit if the ground normal goes in the same direction as the character up
            if (Vector3.Dot(groundHit.normal, transform.up) > 0f)
            {
                isGrounded = true;
                // handle snapping to the ground
                if (groundHit.distance > moveController.skinWidth && currentState != GrappleAirState)
                {
                    moveController.Move(Vector3.down * groundHit.distance);
                }
            }
        }
        else if(Physics.Raycast(groundRay, out groundHit, moveController.height + groundSlantDistance) && !jumpPressed){
            if(Vector3.Dot(groundHit.normal, transform.up) > 0f){
                Debug.Log("On a Slant");
                // Debug.Log("The grounds Normal" + groundHit.normal);
                moveController.Move(groundHit.normal * 20 * Time.deltaTime);
                mSM.CancelMomentum();
            }
        }
    }

    //Actually applies the downwards movement
    void DownwardMovement(){
        Vector3 moveY = new Vector3(0,pStats.GravVel,0) * Time.deltaTime;
        moveController.Move(moveY); 
    }

    //applies Jump values and Variables
    void Jump(){
        if(!pStats.IsPaused){
            //If space/south gamepad button is pressed apply an upwards force to the player
            if (Input.GetAxis("Jump") != 0 && !jumpHeld && curJumpNum < pStats.JumpNum && (mSM.currentState != mSM.SlideState && mSM.currentState != mSM.CrouchState && mSM.currentState != mSM.RagdollState && mSM.currentState != mSM.RecoveringState && currentState != GlidingState))
            {
                if(currentState == WallRunState){
                    AddImpact((GetWallJumpDirection()), pStats.JumpPow * 8.5f);
                    pStats.GravVel = pStats.JumpPow;
                    curJumpNum = 0;
                }
                else{
                    pStats.GravVel = pStats.JumpPow; 
                }
                
                
                curJumpNum++;
                jumpHeld = true;
                jumpPressed = true;
            }
        }

        //If grounded no jumps have been used and coyote Timer is refreshed
        if(isGrounded && pStats.GravVel == 0){
            curCoyJumpTimer = coyJumpTimer;
            curJumpNum = 0;
        } 
        //else start the coyote timer
        else curCoyJumpTimer -= Time.deltaTime;

        //if jump is being held coyote timer is zero
        if(jumpHeld) curCoyJumpTimer = 0;

        //If space/south face gamepad button isn't being pressed then jump is false
        if (!Input.GetButton("Jump")){
           jumpHeld = false;
        }

        if(pStats.GravVel < 0){
            jumpPressed = false;
        }
    }

    //Apply Impact for when force needs to be applied without ragdolling
    public void AddImpact(Vector3 dir, float force){
        if (!IsLocalPlayer) { return; }

        //Normalize direction multiply by force and add it to the impact
        dir.Normalize();
        if (dir.y < 0) dir.y = -dir.y; // reflect down force on the ground
        impact += dir.normalized * force / mass;
    }

    //Dissipates Impact Force
    void DissipateImpact(){
        //if suffiecient impact magnitude is applied then move player
        if (impact.magnitude > 0.2F) moveController.Move(impact * Time.deltaTime);

        // consumes the impact energy each cycle:
        impact = Vector3.Lerp(impact, Vector3.zero, 5*Time.deltaTime);
    }
    ////

    ////Wallrun Functions
    //Checks if they can wallrun
    public bool CanWallRun(){
        float verticalAxis = Input.GetAxisRaw("Vertical");
        return !isGrounded && verticalAxis > 0 && !Physics.Raycast(transform.position, Vector3.down, minimumHeight);
    }
    
    //checks if they can attach
    bool CanAttach(){
        if(jumping)
        {
            elapsedTimeSinceJump += Time.deltaTime;
            if(elapsedTimeSinceJump > jumpDuration)
            {
                elapsedTimeSinceJump = 0;
                jumping = false;
            }
            return false;
        }
        return true;
    }

    //On Wall calulations
    void OnWall(RaycastHit hit){
        float d = Vector3.Dot(hit.normal, Vector3.up);
        if(d >= -normalizedAngleThreshold && d <= normalizedAngleThreshold)
        {
            Vector3 alongWall = Vector3.Cross(hit.normal, Vector3.up);
            float vertical = Input.GetAxisRaw("Vertical");
            //Vector3 alongWall = transform.TransformDirection(Vector3.forward);

            // Debug.DrawRay(transform.position, alongWall.normalized * 10, Color.green);
            // Debug.DrawRay(transform.position, lastWallNormal * 10, Color.magenta);

            Vector3 moveToSet = alongWall * vertical * mSM.PlayerSpeed() * Time.deltaTime;
            Vector3 velNorm = mSM.vel;
            velNorm.Normalize();

            moveToSet = new Vector3(moveToSet.x * -velNorm.x, moveToSet.y, moveToSet.z * -velNorm.z);

            Vector3 moveToSetNorm = moveToSet;
            moveToSetNorm.Normalize();

            if((moveToSetNorm.x < 0 && velNorm.x > 0)){
                moveToSet.x = (moveToSet.x * -velNorm.x);
            }
            else if((moveToSetNorm.x > 0 && velNorm.x < 0) ){
                moveToSet.x = (-moveToSet.x * -velNorm.x);
            }

            if((moveToSetNorm.z < 0 && velNorm.z > 0)){
                moveToSet.z =  (moveToSet.z * -velNorm.z);
            }
            else if((moveToSetNorm.z > 0 && velNorm.z < 0)){
                moveToSet.z =  (-moveToSet.z * -velNorm.z);
            }

            moveToSet.y = 0;
            //

            mSM.vel = moveToSet;
            if(!isWallRunning){
                isWallRunning = true;
            }
            
            if(curJumpNum == mSM.pStats.JumpNum){
                curJumpNum = 0;
            }
            
        }
    }

    //Calculate wall direction
    float CalculateSide(){
        if(isWallRunning)
        {
            Vector3 heading = lastWallPosition - transform.position;
            Vector3 perp = Vector3.Cross(transform.forward, heading);
            float dir = Vector3.Dot(perp, transform.up);
            return dir;
        }
        return 0;
    }

    //The Wallrun Routine itself
    void WallRunRoutine(){ 
        //if (!IsLocalPlayer) { return; }

        isWallRunning = false;

        hits = new RaycastHit[directions.Length];

        if(jumpHeld)
        {
            jumping = true;
        }

        if(CanAttach())
        {
            for(int i=0; i<directions.Length; i++)
            {
                Vector3 dir = transform.TransformDirection(directions[i]);
                Physics.Raycast(transform.position, dir, out hits[i], wallMaxDistance);
                if(hits[i].collider != null)
                {
                    Debug.DrawRay(transform.position, dir * hits[i].distance, Color.green);
                }
                else
                {
                    Debug.DrawRay(transform.position, dir * wallMaxDistance, Color.red);
                }
            }

            if(CanWallRun())
            {
                hits = hits.ToList().Where(h => h.collider != null).OrderBy(h => h.distance).ToArray();
                if(hits.Length > 0)
                {
                    if(hits[0].collider.tag == "WallRun")
                    {
                        OnWall(hits[0]);
                        lastWallPosition = hits[0].point;
                        lastWallNormal = hits[0].normal;
                    }
                }
            }
        }

        if(isWallRunning)
        {
            elapsedTimeSinceWallDetatch = 0;
            elapsedTimeSinceWallAttach += Time.deltaTime;
        }
        else
        {   
            elapsedTimeSinceWallAttach = 0;
            elapsedTimeSinceWallDetatch += Time.deltaTime;
        }
    }

    //The Direction the player jumps on wall detachment
    public Vector3 GetWallJumpDirection(){
        return lastWallNormal * wallBouncing + (transform.up);
    }
    ////

    ////Grapple Functions
    public bool CheckGrapple(){
        if(!pStats.IsPaused){
            if ((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) && pStats.HasGrapple) //If grapple button is hit
            {
                //Debug.Log("Checking Hooks");
                hookPointIndex = FindHookPoint(); //Find the nearest hook point within max distance
                if (hookPointIndex != -1) //If there is a hookpoint
                    {
                        //Debug.Log("Found Hook");
                        hookPoint = hookPoints[hookPointIndex]; //The point we are grappling from
                        return true;
                    }
            }
        }
        return false;
    }

    int FindHookPoint()
    {
        float least = maxGrabDistance;
        int index = -1;
        bool inSightLine;
        bool hookInSight = false;
        for(int i = 0; i<hookPoints.Length; i++)
        {
            distance = Vector3.Distance(gameObject.transform.position, hookPoints[i].transform.position);

            Vector3 screenPoint = cam.WorldToViewportPoint(hookPoints[i].transform.position);
            inSightLine = screenPoint.z > 0 && screenPoint.x > 0 && screenPoint.x < 1 && screenPoint.y > 0 && screenPoint.y < 1;

            if (distance <= least)
            {
                if(inSightLine){
                    index = i;
                    least = distance;
                    hookInSight = true;
                }
                else if(!hookInSight && !inSightLine){
                    index = i;
                }
            }
        }
        return index;
    }

    //lerped grapple release force and dissipation of it
    public void GrappleReleaseForce(){
        if(release){
            currentForcePower *= .90f;
            moveController.Move(postForceDirection * currentForcePower);

            if(currentForcePower < .05) release = false;
        }
    }
    ////

}
