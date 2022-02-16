using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenseStateManager : NetworkBehavior
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
    public GameObject leg; // leg object
    public GameObject legHitbox; // leg hitbox
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
    public AerialStateManager aSM;
    ////

    void Awake(){
        
        ////Initialize Player Components
        moveController = GetComponent<CharacterController>(); // set Character Controller
        rB = GetComponent<Rigidbody>(); //set Rigid Body
        capCol = GetComponent<CapsuleCollider>(); // set Capsule Collider
        capCol.enabled = true;
        animator = GetComponent<Animator>(); // set animator
        ////

        ////Initialize Player Objects
        leg = transform.Find("Leg").gameObject; // Set Leg Object
        legHitbox = leg.transform.Find("LegHitbox").gameObject; // Set Leg Hitbox
        leg.SetActive(false);
        parentObj = transform.parent.gameObject; // set parent object
        ////

        ////Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // set PlayerStats
        mSM = GetComponent<MoveStateManager>(); // set move state manager
        aSM = GetComponent<AerialStateManager>();
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

    private void OnCollisionEnter(Collision collision)
    {
        //if (!IsLocalPlayer) { return; }
        Collider myCollider = collision.contacts[0].thisCollider;
        if (collision.transform.CompareTag("kickable") && myCollider == legHitbox.GetComponent<Collider>()){
            if(collision.gameObject.GetComponent<Rigidbody>().isKinematic == true){
                collision.gameObject.GetComponent<Rigidbody>().isKinematic = false;
            }
            Vector3 direction = this.transform.forward;
            Debug.Log(direction);
            collision.rigidbody.AddForce(direction * pStats.KickPow, ForceMode.Impulse);
        }
        if (collision.transform.CompareTag("destroyable") && myCollider == legHitbox.GetComponent<Collider>()){
            collision.transform.gameObject.GetComponent<BreakableBlock>().damage(pStats.KickPow);
        }
    }
}
