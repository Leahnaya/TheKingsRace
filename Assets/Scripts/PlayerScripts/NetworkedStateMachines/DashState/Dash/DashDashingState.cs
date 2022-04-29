using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDashingState : DashBaseState
{

    Vector3 moveDirection; // direction vector
    const float maxDashTime = .8f; // max dash time
    float dashDistance = 18; // distance to dash
    float dashStoppingSpeed = 0.1f; // how quickly they stop
    float currentDashTime = maxDashTime; // current dash time
    float dashSpeed = 12; // dash speed

    public override void EnterState(DashStateManager dSM, DashBaseState previousState){
        currentDashTime = 0; // resets dash time
        dSM.GetComponent<Animator>().SetBool("isDashing", true);
    }

    public override void ExitState(DashStateManager dSM, DashBaseState nextState){
        moveDirection = Vector3.zero; // resets moveDirection
        dSM.GetComponent<Animator>().SetBool("isDashing", false);
    }

    public override void UpdateState(DashStateManager dSM){
        //if dash timer is active then move player
        if(currentDashTime < maxDashTime){
            moveDirection = dSM.transform.forward * dashDistance;
            currentDashTime += dashStoppingSpeed;
            dSM.pStats.GravVel = 0;
        }

        //if dashtimer runs out then cooldown
        else{
            dSM.SwitchState(dSM.CooldownState);
        }

        //if player becomes incapacitated then cooldown
        if(dSM.mSM.currentState == dSM.mSM.RagdollState || dSM.mSM.currentState == dSM.mSM.SlideState || dSM.mSM.currentState == dSM.mSM.CrouchState || dSM.mSM.currentState == dSM.mSM.CrouchWalkState){
            dSM.SwitchState(dSM.CooldownState);
        }
    }

    public override void FixedUpdateState(DashStateManager dSM){

        //Actually moves the player
        dSM.moveController.Move(moveDirection * Time.deltaTime * dashSpeed);
    }
}
