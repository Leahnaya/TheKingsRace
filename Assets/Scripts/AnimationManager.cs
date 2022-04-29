using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class AnimationManager : NetworkBehaviour
{
    //current priority weight

    //priority 0: priority value of offense and aerial state when not active (should never be current priority)-- is grounded/ incapactiatated, None, and cooldown
    //priority 1: ground state (nothing else active)
    //priority 2: aerial state (nothing above it)
    //priority 3: offense state (nothing above it)
    //priority 4: dash state (nothing above it)
    //priority 5: incapacitated state

    //highest priority/currently active animations;
    private int currentPriority = 1;

    //declare references to all the state managers
    private OffenseStateManager offenseState;
    private NitroStateManager nitroState;
    private MoveStateManager moveState;
    private DashStateManager dashState;
    private AerialStateManager aerialState;

    //declare reference to animator
    private Animator animator;

    //retrieve all state managers
    private void Awake()
    {
        //initialize all state managers references
        offenseState = GetComponent<OffenseStateManager>();
        nitroState = GetComponent<NitroStateManager>();
        moveState = GetComponent<MoveStateManager>();
        dashState = GetComponent<DashStateManager>();
        aerialState = GetComponent<AerialStateManager>();

        animator = GetComponent<Animator>();

    }



    //this function will be called everytime a state is switched. If the new state change would change the current priority number,
    //then the current priorty will changed 
    //Note: if we ever want to have this manager keep track of current animation state and not just a priorty number,
    //then this will need to be changed later (also if we want to add additional animations)
    public void updateCurrentPriority()
    {
        int highestPriority;
        //check if MoveStateManager's current state is incapcitated state
        if (moveState.currentState.GetType() == typeof(MoveRagdollState) || moveState.currentState.GetType() == typeof(MoveRecoveringState))
        {
            highestPriority = 5;

        }
        //if dash manager current state is dashing
        else if (dashState.currentState.GetType() == typeof(DashDashingState))
        {
            highestPriority = 4;
        }

        //check to see if offense manager current state is any state that's doesn't have a weight of 0;
        else if (offenseState.currentState.GetType() != typeof(OffenseIncapacitatedState) && offenseState.currentState.GetType() != typeof(OffenseNoneState) && offenseState.currentState.GetType() != typeof(OffenseCooldownState))
        {
            highestPriority = 3;
        }
        //if current state in aerialManager is not grounded
        else if (aerialState.currentState.GetType() == typeof(AerialJumpingState) || aerialState.currentState.GetType() == typeof(AerialFallingState) || aerialState.currentState.GetType() == typeof(dAerialWallRunState) || aerialState.currentState.GetType() == typeof(AerialGrappleAirState) || aerialState.currentState.GetType() == typeof(AerialGlidingState))
        {
            highestPriority = 2;
        }
        //ground movement is the only state with a animation right now
        else
        {
            highestPriority = 1;
        }
        //if highest priorty does not equal current, then a state changed has resulted in a new higher priorty animation
        if (highestPriority != currentPriority)
        {
            currentPriority = highestPriority;
            animator.SetInteger("currentPriority", currentPriority);

        }
    }
    //function will set the currentPriorty to the newPriority arguement. 
    //Note: this function is mostly meant to exist if we need to force a new priorty for some reason, otherwise lgoci will be handled in check
    public void SetCurrentPriorityState(int newPriority)
    {
        //set var to new prio
        currentPriority = newPriority;
        //set animator priority var
        animator.SetInteger("currentPriority", currentPriority);
    }

    //return priority of a given state (stubbed out for not as not needed for current implementation but may be desired later)
    public int GetCurrentPriortyValue()
    {
        return 0;
    }

    //retrieve highest priority value (current priorty should almost always be 
    public int GetHighestPriorityValue()
    {
        return currentPriority;
    }

}

