using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MoveStateManager : NetworkBehaviour
{
    ////Player States
    public MoveBaseState currentState;
    public MoveBaseState previousState;

    //WASD States
    public MoveIdleState IdleState = new MoveIdleState();
    public MoveWalkState WalkState = new MoveWalkState();
    public MoveJogState JogState = new MoveJogState();
    public MoveRunState RunState = new MoveRunState();

    //Slide States
    public MoveSlideState SlideState = new MoveSlideState();
    public MoveCrouchState CrouchState = new MoveCrouchState();
    public MoveCrouchWalkState CrouchWalkState = new MoveCrouchWalkState();

    //Incapitated States
    public MoveRagdollState RagdollState = new MoveRagdollState();
    public MoveRecoveringState RecoveringState = new MoveRecoveringState();

    //Grapple States
    public MoveGrappleAirState GrappleAirState = new MoveGrappleAirState();
    ////

    ////Objects Sections
    private GameObject parentObj; // Parent object
    public Camera cam; // Camera object
    ////

    ////Components Section
    public CharacterController moveController; // Character Controller
    public Rigidbody rB; // Players Rigidbody
    public CapsuleCollider capCol; // Players Capsule Collider
    private Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public AerialStateManager aSM;
    ////

    ////State Transition Variables
    public float idleLimit = .3f;
    public float walkLimit = 10.0f;
    public float jogLimit = 20f;
    public float runLimit = 30f;
    ////

    ////Player Variables Section
    //Speed Variables
    public Vector3 vel; // moveZ + moveX
    private Vector3 moveZ; // Local Horizontal Vector
    private Vector3 moveX; // Local Vertical Vector
    public Vector3 driftVel; // Lerped Movement Vector
    public float calculatedCurVel; // calculated current vel using driftVel

    //Slide Variables
    public Vector3 slideUp; // Slide upwards direction

    //Camera Variables
    private Vector3 camRotation; // cameras camera rotation vector
    [Range(-45, -15)]
    public int minAngle = -30; // minimum downwards cam angle
    [Range(30, 80)]
    public int maxAngle = 45; // Max upwards cam angle
    [Range(50, 500)]
    public int sensitivity = 200; // Camera sensitivity

    //Ragdoll Variables
    public Vector3 dirHit; // Direction hit
    public float distToGround; // distance to ground
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
        aSM = GetComponent<AerialStateManager>();
        ////
    }

    // Start is called before the first frame update
    void Start()
    {
        //players starting state
        currentState = IdleState;
        previousState = IdleState;
        currentState.EnterState(this, previousState);

        //Slide Upwards Variable
        slideUp = GetComponentInParent<Transform>().up; // get parents up direction
        distToGround = GetComponent<Collider>().bounds.extents.y; // set players distance to ground

        Cursor.lockState = CursorLockMode.Locked; // Lock cursor on start if you are the local player
    }

    // Update is called once per frame
    void Update()
    {
        //calculates vel using driftVel will need to be relocated
        calculatedCurVel = driftVel.magnitude * 50f;

        GoToGrapple();

        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);

        if(cam.enabled) Rotation();  
        else Debug.Log("Cam Disabled");
    }

    public void SwitchState(MoveBaseState state){
        
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        currentState.EnterState(this, previousState);
    }


    ////Broad Functions
    //Player Speed Calculator
    public float PlayerSpeed(){
        //If nothing is pressed speed is 0
        if ((Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f))
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
        else if (pStats.CurVel >= pStats.MaxVel)
        {
            pStats.CurVel = pStats.MaxVel;
            return pStats.CurVel;

        }
        //case if somehow they escape this check
        else{
            Debug.Log("Something has gone wrong with the PlayerSpeed()");
            return -1;
        }
    }

    //Wasd movement using player speed
    public void DirectionalMovement(){
        //Keyboard inputs
        //Checks if movement keys have been pressed and calculates correct vector
        moveX = transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * PlayerSpeed();
        moveZ = transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * PlayerSpeed();

        vel = moveX + moveZ;
        Vector3 moveXZ = new Vector3(vel.x, 0, vel.z);
        driftVel = Vector3.Lerp(driftVel, moveXZ, pStats.Traction * Time.deltaTime);
        
        //Actually move he player
        moveController.Move(driftVel);
    }

    //Slide movement
    public void SlideMovement(){

        moveX = Vector3.zero;
        moveZ = Vector3.zero;

        //Adds vectors based on movement keys and other conditions to check what the
        //player vector should be under the circumstances
        vel = moveX + moveZ;
        Vector3 moveXZ = new Vector3(vel.x, 0, vel.z);
        driftVel = Vector3.Lerp(driftVel, moveXZ, pStats.Traction * Time.deltaTime);
        
        //Actually move he player
        moveController.Move(driftVel);
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

    public void GetHit(Vector3 dir, float force){
        //if (!IsLocalPlayer) { return; }
        dir.Normalize();
        dirHit = dir * force;
        SwitchState(RagdollState);
    }

    public void CancelMomentum(){
        pStats.CurVel = 0;
        vel = Vector3.zero;
        moveX = Vector3.zero;
        moveZ = Vector3.zero;
        driftVel = Vector3.zero;
    }

    void GoToGrapple(){
        if(aSM.currentState == aSM.GrappleAirState && (currentState != SlideState && currentState != RagdollState && currentState != RecoveringState)){
            SwitchState(GrappleAirState);
        }
    }
}
