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
    public GameObject playerModel; // player visual model
    public Camera cam; // Camera object
    ////

    ////Components Section
    public CharacterController moveController; // Character Controller
    public Rigidbody rB; // Players Rigidbody
    public CapsuleCollider capCol; // Players Capsule Collider
    public Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public AerialStateManager aSM;
    //// AnimatorManagerScript
    private AnimationManager animationManager;
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
    public int layerMask; // LayerMask to exclude player
    RaycastHit wallHitTop, wallHitBot, wallExitTop, wallExitBot; // Raycast for momentum loss on wall hit
    private Vector3 lastVel; // previous vel
    private bool firstWallHit = false; // hit the wall for the first time
    private float tempCurVel;
    private bool setTempVel = false;

    //Camera Variables
    private Vector3 camRotation; // cameras camera rotation vector
    [Range(-45, -15)]
    public int minAngle = -18; // minimum downwards cam angle
    [Range(30, 80)]
    public int maxAngle = 30; // Max upwards cam angle
    [Range(50, 500)]
    public int sensitivity = 200; // Camera sensitivity

    //Ragdoll Variables
    public Vector3 dirHit; // Direction hit
    public float distToGround; // distance to ground
    ////

    //audio set
    public AudioSource slideAudio;
    public AudioSource skateAudio;
    public AudioSource dirtStep1, dirtStep2, dirtStep3, dirtStep4, dirtStep5, dirtStep6;
    public AudioSource rainStep1, rainStep2, rainStep3, rainStep4, rainStep5, rainStep6;
    public AudioSource snowStep1, snowStep2, snowStep3, snowStep4, snowStep5, snowStep6;
    bool hasPlayed;

    void Awake(){

        ////Initialize Player Components
        moveController = GetComponent<CharacterController>(); // set Character Controller
        rB = GetComponent<Rigidbody>(); //set Rigid Body
        capCol = GetComponent<CapsuleCollider>(); // set Capsule Collider
        capCol.enabled = true;
        parentObj = transform.parent.gameObject; // set parent object
        animator = GetComponent<Animator>(); // set animator
        animationManager = GetComponent<AnimationManager>();
        ////

        ////Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // set PlayerStats
        aSM = GetComponent<AerialStateManager>(); // aerial state manager
        ////

        ////Initialize Variables
        layerMask = (1 << 17);
        //layerMask = ~layerMask;
        ////

    }

    // Start is called before the first frame update
    void Start()
    {
        //players starting state
        currentState = IdleState;
        previousState = IdleState;
        currentState.EnterState(this, previousState);

        distToGround = GetComponent<Collider>().bounds.extents.y; // set players distance to ground

        if (!IsLocalPlayer) { return; }
        Cursor.lockState = CursorLockMode.Locked; // Lock cursor on start if you are the local player
        pStats.CurTraction = pStats.Traction;
        pStats.CurAcc = pStats.Acc;
    }

    // Update is called once per frame
    void Update()
    {
        if(currentState == null){
                    //players starting state
            currentState = IdleState;
            previousState = IdleState;
            currentState.EnterState(this, previousState);

            distToGround = GetComponent<Collider>().bounds.extents.y; // set players distance to ground

            if (!IsLocalPlayer) { return; }
            Cursor.lockState = CursorLockMode.Locked; // Lock cursor on start if you are the local player
            pStats.CurTraction = pStats.Traction;
            pStats.CurAcc = pStats.Acc;
        }

        if (!IsLocalPlayer) { return; }

        //calculates vel using driftVel will need to be relocated
        calculatedCurVel = driftVel.magnitude * 50f;


        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        if (!IsLocalPlayer) { return; }

        //if camera is enabled then rotate
        if(cam.enabled) Rotation();  
        else Debug.Log("Cam Disabled");

        if(moveController.enabled){
            ApplyWind(pStats.WindOn); 
        }
        
        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(MoveBaseState state){
        if (!IsLocalPlayer) { return; }
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        animationManager.updateCurrentPriority();
        currentState.EnterState(this, previousState);
    }


    ////Broad Functions
    //Player Speed Calculator
    public float PlayerSpeed(){
        //If nothing is pressed speed is 0 set a tempVel
        if (((Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f) && !setTempVel) || pStats.IsPaused)
        {
            if(pStats.CurVel > 0.0f){
                tempCurVel = pStats.CurVel;
                setTempVel = true;
            }

            pStats.CurVel = 0.0f;
            return pStats.CurVel;
        }
        //
        else if((Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f) && setTempVel){

            tempCurVel -= .35f;
            if(tempCurVel <= 0){
                setTempVel = false;
                tempCurVel = 0;
            }

            pStats.CurVel = 0.0f;
            return pStats.CurVel;
        }
        else if(tempCurVel > 0){

            if(tempCurVel < pStats.MinVel){

                pStats.CurVel = pStats.MinVel;
                tempCurVel = 0;
                setTempVel = false;
                return pStats.MinVel;
            }

            pStats.CurVel = tempCurVel;
            tempCurVel = 0;
            setTempVel = false;

            return pStats.CurVel;
        }
        //If current speed is below min when pressed set to minimum speed
        else if (pStats.CurVel < pStats.MinVel || firstWallHit == true)
        {
            pStats.CurVel = pStats.MinVel;
            return pStats.MinVel;
        }
        // while the speed is below max speed slowly increase it
        else if ((pStats.CurVel >= pStats.MinVel) && (pStats.CurVel < pStats.MaxVel))
        {
            pStats.CurVel += pStats.CurAcc;
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

    
    public void CrouchMovement(){
        //Keyboard inputs
        //Checks if movement keys have been pressed and calculates correct vector
        if(currentState == CrouchWalkState){
            float curSpeed = PlayerSpeed();
            if(curSpeed != 0){
                moveX = transform.right * Input.GetAxis("Horizontal") * Time.deltaTime * 2;
                moveZ = transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * 2;
            }
            else{
                moveX = Vector3.zero;
                moveZ = Vector3.zero;
            }

            vel = moveX + moveZ;
            Vector3 moveXZ = new Vector3(vel.x, 0, vel.z);

            driftVel = Vector3.Lerp(driftVel, moveXZ, 10 * Time.deltaTime);
            if(currentState == GrappleAirState){
                driftVel = Vector3.zero;
            }
        
            if(driftVel != Vector3.zero){
                playerModel.transform.rotation = Quaternion.LookRotation(driftVel.normalized);  
            }

            //Actually move he player
            moveController.Move(driftVel); 
            }
        else{
            moveController.Move(Vector3.zero); 
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
        
        driftVel = Vector3.Lerp(driftVel, moveXZ, pStats.CurTraction * Time.deltaTime);
        if(currentState == GrappleAirState){
            driftVel = Vector3.zero;
        }
        
                //Raycast offset
        Vector3 rayOffset = driftVel - lastVel;
        
        
        //Check if wall is in direction player is moving
        if (((Physics.Raycast(gameObject.transform.position + new Vector3(0,.4f,0) + rayOffset, driftVel.normalized, out wallHitBot, .35f, layerMask) == true) || ((currentState != SlideState || currentState != CrouchState || currentState == CrouchWalkState) && (Physics.Raycast(gameObject.transform.position + new Vector3(0,2.2f,0), driftVel.normalized, out wallHitTop, .35f, layerMask) == true))) && !firstWallHit){
            CancelMomentum();
            //Debug.Log(wallHitBot.collider.name);
            firstWallHit = true;
        }
        if(((Physics.Raycast(gameObject.transform.position + new Vector3(0,.4f,0) + rayOffset, driftVel.normalized, out wallExitBot, .3f, layerMask) == false) && ((currentState == SlideState || currentState == CrouchState || currentState == CrouchWalkState) || (Physics.Raycast(gameObject.transform.position + new Vector3(0,2.2f,0), driftVel.normalized, out wallExitTop, .3f, layerMask) == false))) && firstWallHit){
            firstWallHit = false;
        }

        Debug.DrawRay(gameObject.transform.position + new Vector3(0,.4f,0) + rayOffset, driftVel.normalized * .5f, Color.red);
        Debug.DrawRay(gameObject.transform.position + new Vector3(0,2.2f,0) + rayOffset, driftVel.normalized * .5f, Color.red);
        

        if(driftVel != Vector3.zero){
            playerModel.transform.rotation = Quaternion.LookRotation(driftVel.normalized);  
        }

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
        driftVel = Vector3.Lerp(driftVel, moveXZ, pStats.CurTraction * Time.deltaTime);
        

        //True false set here to stop the audio from playing each time = to FPS
        if (hasPlayed == false)
        {
             hasPlayed = true;
             slideAudio.Play();
        }

        else
        {
         hasPlayed = false;
        }

        //Actually move he player
        moveController.Move(driftVel);
        
    }

    //Camera and player rotation
    private void Rotation(){
        //If moveController is enabled allow Camera control
        if(moveController.enabled && !pStats.IsPaused){
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

    //Get hit into a ragdoll
    public void GetHit(Vector3 dir, float force){
        if (!IsLocalPlayer) { return; }
        dir.Normalize();
        dirHit = dir * force;
        SwitchState(RagdollState);
    }

    //remove player momentum
    public void CancelMomentum(){
        pStats.CurVel = 0;
        vel = Vector3.zero;
        moveX = Vector3.zero;
        moveZ = Vector3.zero;
        driftVel = Vector3.zero;
    }

    //Apply Wind movement to the player
    public void ApplyWind(bool wind){
        if(wind){
                moveController.Move(pStats.WindDirection.normalized * .2f); 
        }
    }

    ////

}
