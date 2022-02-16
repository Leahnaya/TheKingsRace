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
    ////
    
    ////Objects Sections
    GameObject parentObj; // Parent object
    public Camera cam; // Camera object
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
}
