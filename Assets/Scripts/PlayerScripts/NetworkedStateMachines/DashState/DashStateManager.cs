using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class DashStateManager : NetworkBehaviour
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

    ////Components Section
    public CharacterController moveController; // Character Controller
    public Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public MoveStateManager mSM; // movement state manager
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
        //driver = GameObject.Find("Canvas").GetComponent<CoolDown>();
        ////

        ////Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // set PlayerStats
        mSM = GetComponent<MoveStateManager>(); // set move state manager
        animationManager = GetComponent<AnimationManager>();
        ////
    }

    void Start(){
        //players starting state
        currentState = NoneState;
        previousState = NoneState;
        currentState.EnterState(this, previousState);
    }

    void Update(){

        if (!IsLocalPlayer) { return; }
        
        //calls any logic in the update state from current state
        currentState.UpdateState(this);
    }

    void FixedUpdate(){

        if (!IsLocalPlayer) { return; }

        //calls any logic in the fixed update state from current state
        currentState.FixedUpdateState(this);
    }

    public void SwitchState(DashBaseState state){
        
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        animationManager.updateCurrentPriority();
        currentState.EnterState(this, previousState);
    }
}
