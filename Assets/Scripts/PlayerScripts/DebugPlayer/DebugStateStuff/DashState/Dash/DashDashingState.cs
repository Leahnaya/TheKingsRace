using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashDashingState : DashBaseState
{

    Vector3 moveDirection;
    const float maxDashTime = 1.0f;
    float dashDistance = 10;
    float dashStoppingSpeed = 0.1f;
    float currentDashTime = maxDashTime;
    float dashSpeed = 12;

    public override void EnterState(DashStateManager dSM, DashBaseState previousState){
        currentDashTime = 0;
    }

    public override void ExitState(DashStateManager dSM, DashBaseState nextState){
        moveDirection = Vector3.zero;
    }

    public override void UpdateState(DashStateManager dSM){
        if(currentDashTime < maxDashTime){
            moveDirection = dSM.transform.forward * dashDistance;
            currentDashTime += dashStoppingSpeed;
        }
        else{
            dSM.SwitchState(dSM.CooldownState);
        }

        if(dSM.mSM.currentState == dSM.mSM.RagdollState || dSM.mSM.currentState == dSM.mSM.SlideState || dSM.mSM.currentState == dSM.mSM.CrouchState || dSM.mSM.currentState == dSM.mSM.CrouchWalkState){
            dSM.SwitchState(dSM.CooldownState);
        }
    }

    public override void FixedUpdateState(DashStateManager dSM){
        dSM.moveController.Move(moveDirection * Time.deltaTime * dashSpeed);
    }
}
