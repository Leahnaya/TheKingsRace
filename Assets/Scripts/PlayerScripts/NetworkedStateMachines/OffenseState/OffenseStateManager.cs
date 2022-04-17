using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using MLAPI.Messaging;

public class OffenseStateManager : NetworkBehaviour
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
    public Animator animator; // Animation Controller
    ////

    ////Scripts Section
    public PlayerStats pStats; // Player Stats
    public MoveStateManager mSM; // move state manager
    public AerialStateManager aSM; // aerial state manager
    ////

    //// AnimatorManagerScript
    private AnimationManager animationManager;
    ////

    //// Variables Section
    private GameObject[] players;

    void Awake(){
        
        ////Initialize Player Components
        moveController = GetComponent<CharacterController>(); // set Character Controller
        rB = GetComponent<Rigidbody>(); //set Rigid Body
        capCol = GetComponent<CapsuleCollider>(); // set Capsule Collider
        capCol.enabled = true;
        animator = GetComponent<Animator>(); // set animator
        animationManager = GetComponent<AnimationManager>();
        ////

        ////Initialize Player Objects
        leg = transform.Find("Leg").gameObject; // Set Leg Object
        legHitbox = leg.transform.Find("LegHitbox").gameObject; // Set Leg Hitbox
        leg.SetActive(false);
        ////

        ////Initialize Scripts
        pStats = GetComponent<PlayerStats>(); // set PlayerStats
        mSM = GetComponent<MoveStateManager>(); // set move state manager
        aSM = GetComponent<AerialStateManager>(); // set aerial state manager
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

    public void SwitchState(OffenseBaseState state){
        if (!IsLocalPlayer) { return; }
        currentState.ExitState(this, state);

        //Sets the previous State
        previousState = currentState;

        //updates current state and calls logic for entering
        currentState = state;
        animationManager.updateCurrentPriority();
        currentState.EnterState(this, previousState);
    }

    //Collision checker for the leg
    private void OnCollisionEnter(Collision collision)
    {
        if (!IsLocalPlayer) { return; }
        Collider myCollider = collision.contacts[0].thisCollider;

        // Kickable items must be handled through the server since they need to modify the NetworkTransform
        if (collision.transform.CompareTag("kickable") && myCollider == legHitbox.GetComponent<Collider>()) {
            Vector3 direction = this.transform.forward;
            //ulong prefabHash = collision.gameObject.GetComponent<NetworkObject>().PrefabHash;
            ulong netObjID = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
            ApplyKickServerRPC(direction, netObjID);
        }

        if(collision.transform.CompareTag("ArcherTarget") && myCollider == legHitbox.GetComponent<Collider>()){
            Vector3 direction = this.transform.forward;

            ulong netObjID = collision.gameObject.transform.root.GetComponent<NetworkObject>().NetworkObjectId;
            if(netObjID != this.transform.root.GetComponent<NetworkObject>().NetworkObjectId){
                Debug.Log("NetObjID of Kicked Player: " + netObjID);
                ApplyKickServerRPC(direction, netObjID); 
            }
            
        }

        if (collision.transform.CompareTag("destroyable") && myCollider == legHitbox.GetComponent<Collider>()){
            //collision.transform.gameObject.GetComponent<BreakableBlock>().damage(pStats.KickPow);
            BreakBlockServerRPC(collision.transform.gameObject.GetComponent<NetworkObject>().NetworkObjectId);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void BreakBlockServerRPC(ulong NetObjID) {
        GameObject[] destroyables = GameObject.FindGameObjectsWithTag("destroyable");

        foreach (GameObject destroyable in destroyables) {
            if (destroyable.GetComponent<NetworkObject>() != null) {
                if (destroyable.GetComponent<NetworkObject>().NetworkObjectId == NetObjID) {
                    destroyable.GetComponent<NetworkObject>().Despawn(true);
                }
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void ApplyKickServerRPC(Vector3 direction, ulong netObjId) {
        GameObject[] kickables = GameObject.FindGameObjectsWithTag("kickable");

        // Get all players in the scene
        GameObject[] playableCharacters = GameObject.FindGameObjectsWithTag("ArcherTarget");

        foreach (GameObject kickedItem in kickables) { 
            // First check to make sure this is the item we kicked
            if (kickedItem.GetComponent<NetworkObject>() != null && kickedItem.GetComponent<NetworkObject>().NetworkObjectId == netObjId) {
                // First turn off kinematic
                if (kickedItem.gameObject.GetComponent<Rigidbody>().isKinematic == true) {
                    kickedItem.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                }

                // Then apply the force
                kickedItem.gameObject.GetComponent<Rigidbody>().AddForce(direction * pStats.KickPow, ForceMode.Impulse);

                // Then return since we only kicked one item and don't need to check the remainder of the items
                return;
            }
        }

        foreach(GameObject character in playableCharacters){
            if(character.transform.root.GetComponent<NetworkObject>() != null && character.transform.root.GetComponent<NetworkObject>().NetworkObjectId == netObjId){

                ulong kickedPlayerClientID = character.transform.root.GetComponent<NetworkObject>().OwnerClientId;

                //Apply kick to other player
                ClientRpcParams clientRpcParams = new ClientRpcParams
                {
                    Send = new ClientRpcSendParams
                    {
                        TargetClientIds = new ulong[] { kickedPlayerClientID }
                    }
                };

                ApplyKickToPlayerClientRPC(kickedPlayerClientID, direction, clientRpcParams);
                return;
            }

        }
    }

    [ClientRpc]
    private void ApplyKickToPlayerClientRPC(ulong playerToKickID, Vector3 direction, ClientRpcParams clientRpcParams) {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject player in players) { 
            if (player.GetComponent<NetworkObject>().OwnerClientId == playerToKickID) {
                player.GetComponentInChildren<MoveStateManager>().GetHit(direction, 20);
                return;
            }
        }
    }
}
