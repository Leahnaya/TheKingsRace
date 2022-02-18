using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dDashNoneState : dDashBaseState
{
    public override void EnterState(dDashStateManager dSM, dDashBaseState previousState){

    }

    public override void ExitState(dDashStateManager dSM, dDashBaseState nextState){

    }

    public override void UpdateState(dDashStateManager dSM){

        //checks if player has dash
        if(dSM.pStats.HasDash){
            
            //if incapacitated then incapacitated
            if(dSM.mSM.currentState == dSM.mSM.RagdollState || dSM.mSM.currentState == dSM.mSM.SlideState || dSM.mSM.currentState == dSM.mSM.CrouchState || dSM.mSM.currentState == dSM.mSM.CrouchWalkState){
                dSM.SwitchState(dSM.IncapacitatedState);
            } 
            
            //if R key then Dashing
            else if ((Input.GetKeyDown(KeyCode.R) || Input.GetAxis("Dash") != 0)){
                dSM.SwitchState(dSM.DashingState);
            }

        }
        
    }

    public override void FixedUpdateState(dDashStateManager dSM){

    }
}
