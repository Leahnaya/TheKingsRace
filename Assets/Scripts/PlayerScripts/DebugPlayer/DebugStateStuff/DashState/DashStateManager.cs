using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashStateManager : MonoBehaviour
{
    ////Player States
    public DashBaseState currentState;
    public DashBaseState previousState;

    //Dash States
    public DashNoneState NoneState = new DashNoneState();
    public DashIncapacitatedState IncapacitatedState = new DashIncapacitatedState();
    public DashCooldownState CooldownState = new DashCooldownState();
    public DashDashingState DashingState = new DashDashingState();
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
    public CoolDown driver;
    ////

    ////Items Section
    public SpecialItem dashItem;
    ////

    void Awake(){
        
        ////Initialize Player Components
        moveController = GetComponent<CharacterController>(); // set Character Controller
        rB = GetComponent<Rigidbody>(); //set Rigid Body
        capCol = GetComponent<CapsuleCollider>(); // set Capsule Collider
        capCol.enabled = true;
        parentObj = transform.parent.gameObject; // set parent object
        animator = GetComponent<Animator>(); // set animator
        //driver = GameObject.Find("Canvas").GetComponent<CoolDown>();
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

    public void SwitchState(DashBaseState state){
        
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        currentState.EnterState(this, previousState);
    }
}
