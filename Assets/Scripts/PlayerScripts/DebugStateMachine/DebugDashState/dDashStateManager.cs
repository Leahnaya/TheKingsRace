using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class dDashStateManager : NetworkBehaviour
{
    ////Player States
    public dDashBaseState currentState;
    public dDashBaseState previousState;

    //Dash States
    public dDashNoneState NoneState = new dDashNoneState();
    public dDashIncapacitatedState IncapacitatedState = new dDashIncapacitatedState();
    public dDashCooldownState CooldownState = new dDashCooldownState();
    public dDashDashingState DashingState = new dDashDashingState();
    ////

    ////Components Section
    public CharacterController moveController; // Character Controller
    public Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public dMoveStateManager mSM; // movement state manager
    public CoolDown driver; // cooldown driver
    //// AnimatorManagerScript
    private AnimationManager animationManager;
    ////

    ////Items Section
    public SpecialItem dashItem; // dash item
    ////

    void Awake(){
        
        ////Initialize Player Components
        moveController = GetComponent<CharacterController>(); // set Character Controller
        animator = GetComponent<Animator>(); // set animator
        animationManager = GetComponent<AnimationManager>();
        //driver = GameObject.Find("Canvas").GetComponent<CoolDown>();
        ////

        ////Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // set PlayerStats
        mSM = GetComponent<dMoveStateManager>(); // set move state manager
        ////
    }

    void Start(){
        //players starting state
        currentState = NoneState;
        previousState = NoneState;
        currentState.EnterState(this, previousState);
    }

    void Update(){

        //if (!IsLocalPlayer) { return; }
        
        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        //if (!IsLocalPlayer) { return; }

        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(dDashBaseState state){
        
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        //update current animation priorty in animation manager
        animationManager.updateCurrentPriority();
        currentState.EnterState(this, previousState);
    }
}
