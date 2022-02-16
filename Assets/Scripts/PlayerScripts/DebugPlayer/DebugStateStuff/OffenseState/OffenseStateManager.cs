using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenseStateManager : MonoBehaviour
{
    ////Player States
    public OffenseBaseState currentState;
    public OffenseBaseState previousState;

    //Offense States
    public OffenseNoneState NoneState = new OffenseNoneState();
    public OffenseIncapacitatedState IncapacitatedState = new OffenseIncapacitatedState();
    public OffenseCooldownState CooldownState = new OffenseCooldownState();

    //Kick&Punch States
    public OffenseKickState KickState = new OffenseKickState();
    public OffenseAirKickState AirKickState = new OffenseAirKickState();
    public OffensePunchState PunchState = new OffensePunchState();
    public OffenseAirPunchState AirPunchState = new OffenseAirPunchState();
    ////
    
    ////Objects Sections
    GameObject parentObj; // Parent object
    ////

    ////Components Section
    public CharacterController moveController; // Character Controller
    Rigidbody rB; // Players Rigidbody
    CapsuleCollider capCol; // Players Capsule Collider
    Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public MoveStateManager mSM;
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
        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){
        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(OffenseBaseState state){
        
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        currentState.EnterState(this, previousState);
    }
}
