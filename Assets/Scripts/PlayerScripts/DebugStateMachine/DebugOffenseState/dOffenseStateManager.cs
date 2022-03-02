using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class dOffenseStateManager : NetworkBehaviour
{
    ////Player States
    public dOffenseBaseState currentState;
    public dOffenseBaseState previousState;

    //Offense States
    public dOffenseNoneState NoneState = new dOffenseNoneState();
    public dOffenseIncapacitatedState IncapacitatedState = new dOffenseIncapacitatedState();
    public dOffenseCooldownState CooldownState = new dOffenseCooldownState();

    //Kick&Punch States
    public dOffenseKickState KickState = new dOffenseKickState();
    public dOffenseAirKickState AirKickState = new dOffenseAirKickState();
    public dOffensePunchState PunchState = new dOffensePunchState();
    public dOffenseAirPunchState AirPunchState = new dOffenseAirPunchState();
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
    public dMoveStateManager mSM; // move state manager
    public dAerialStateManager aSM; // aerial state manager
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
        mSM = GetComponent<dMoveStateManager>(); // set move state manager
        aSM = GetComponent<dAerialStateManager>(); // set aerial state manager
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

    public void SwitchState(dOffenseBaseState state){
        
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        currentState.EnterState(this, previousState);
    }

    //Collision checker for the leg
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

        // if(collision.transform.CompareTag("ArcherTarget") && myCollider == legHitbox.GetComponent<Collider>()){
        //     Vector3 direction = this.transform.forward;
        //     collision.gameObject.GetComponent<dMoveStateManager>().GetHit(this.transform.forward, 10);
        // }
    }
}
