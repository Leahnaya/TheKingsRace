using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroStateManager : MonoBehaviour
{
    ////Player States
    public NitroBaseState currentState;
    public NitroBaseState previousState;
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
