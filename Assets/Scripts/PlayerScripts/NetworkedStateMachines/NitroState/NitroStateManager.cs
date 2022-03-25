using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class NitroStateManager : NetworkBehaviour
{
    ////Player States
    public NitroBaseState currentState;
    public NitroBaseState previousState;

    //Nitro States
    public NitroNoneState NoneState = new NitroNoneState();
    public NitroIncapacitatedState IncapacitatedState = new NitroIncapacitatedState();
    public NitroCooldownState CooldownState = new NitroCooldownState();
    public NitroNitroingState NitroingState = new NitroNitroingState();
    ////

    ////Objects Sections
    public GameObject parentObj; // Parent object
    ////

    ////Components Section
    public CharacterController moveController; // Character Controller
    Rigidbody rB; // Players Rigidbody
    CapsuleCollider capCol; // Players Capsule Collider
    Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public MoveStateManager mSM; // move state manager
    public CoolDown driver; // cooldown driver
    //// AnimatorManagerScript
    private AnimationManager animationManager;
    ////

    ////Items Section
    public SpecialItem nitroItem; // nitro item
    ////

    ////Variables Section
    public float nitroVelBoost = 40;
    public float nitroAccBoost = .4f;

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
        mSM = GetComponent<MoveStateManager>(); // set move state manager
        ////
    }

    void Start(){
        //players starting state
        currentState = NoneState;
        previousState = NoneState;
        currentState.EnterState(this, previousState);
    }

    void Update(){
        
        if(currentState == null){
            //players starting state
            currentState = NoneState;
            previousState = NoneState;
            currentState.EnterState(this, previousState);
        }

        if (!IsLocalPlayer) { return; }

        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        if (!IsLocalPlayer) { return; }

        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(NitroBaseState state){
        
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        animationManager.updateCurrentPriority();
        currentState.EnterState(this, previousState);
    }
}
