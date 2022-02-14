using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialStateManager : MonoBehaviour
{
    ////Player States
    AerialBaseState currentState;
    public AerialBaseState previousState;

    //Aerial States
    public AerialFallingState FallingState = new AerialFallingState();
    public AerialGlidingState GlidingState = new AerialGlidingState();
    public AerialGroundedState GroundedState = new AerialGroundedState();
    public AerialJumpingState JumpingState = new AerialJumpingState();
    ////

    ////Objects Sections
    private GameObject parentObj; // Parent object
    public Camera cam; // Camera object
    ////

    ////Components Section
    public CharacterController moveController; // Character Controller
    private Rigidbody rB; // Players Rigidbody
    private CapsuleCollider capCol; // Players Capsule Collider
    private Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public MoveStateManager mSM;
    ////

    ////Variables Section
    //Jump Variables
    public int curJumpNum; // current Jumps Used
    public bool jumpHeld; // Jump is Held
    private bool jumpPressed; // Jamp was pressed
    public float coyJumpTimer = 0.1f; // Default Coyote Jump time
    public float curCoyJumpTimer = 0.1f; // current Coyote Jump time
    public float lowJumpMultiplier; // Short jump multiplier
    public float fallMultiplier; // High Jump Multiplier

    //Gravity Variables//
    private float maxG = -100; // max downwards velocity

    //Ground Check
    public bool isGrounded; // is player grounded
    public float groundCheckDistance = 0.05f; // offset distance to check ground
    private const float jumpGroundingPreventionTime = 0.2f; // delay so player doesn't get snapped to ground while jumping
    private const float groundCheckDistanceInAir = 0.07f; // How close we have to get to ground to start checking for grounded again
    private Ray groundRay; // ground ray
    private RaycastHit groundHit; // ground raycast
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
        mSM = GetComponent<MoveStateManager>();
        ////
    }

    void Start(){
        //players starting state
        currentState = GroundedState;
        previousState = GroundedState;
        currentState.EnterState(this);
    }

    void Update(){
        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        if(moveController.enabled){
            Jump();
            GroundCheck();
            DownwardMovement();
        }
        else{
            Debug.Log(currentState);
            //Gravity without moveController
            pStats.GravVel -= pStats.PlayerGrav * Time.deltaTime;
            rB.AddForce(new Vector3(0,pStats.GravVel,0));
        }
        

        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(AerialBaseState state){
        
        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        currentState.EnterState(this);
    }

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
            if((isGrounded == false && curCoyJumpTimer <= 0)/* || grapple.isGrappled*/){
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
                if (groundHit.distance > moveController.skinWidth /* && !grapple.isGrappled*/)
                {
                    moveController.Move(Vector3.down * groundHit.distance);
                }
            }
        }
    }

    private void DownwardMovement(){
        Vector3 moveY = new Vector3(0,pStats.GravVel,0) * Time.deltaTime;

        moveController.Move(moveY);
    }

    private void Jump(){
        //If space/south gamepad button is pressed apply an upwards force to the player
        if (Input.GetAxis("Jump") != 0 && !jumpHeld && curJumpNum < pStats.JumpNum)
        {
            pStats.GravVel = pStats.JumpPow;
            
            curJumpNum++;
            jumpHeld = true;
            jumpPressed = true;
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
        if (Input.GetAxis("Jump") == 0){
           jumpHeld = false;
        }

        if(pStats.GravVel < 0){
            jumpPressed = false;
        }
    }

        /*
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
        */
}
