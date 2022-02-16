using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashNoneState : DashBaseState
{
    public override void EnterState(DashStateManager dSM, DashBaseState previousState){

    }

    public override void ExitState(DashStateManager dSM, DashBaseState nextState){

    }

    public override void UpdateState(DashStateManager dSM){
        if(dSM.pStats.HasDash){
            if ((Input.GetKeyDown(KeyCode.R) || Input.GetAxis("Dash") != 0)){
                dSM.SwitchState(dSM.DashingState);
            }

            if(dSM.mSM.currentState == dSM.mSM.RagdollState || dSM.mSM.currentState == dSM.mSM.SlideState || dSM.mSM.currentState == dSM.mSM.CrouchState || dSM.mSM.currentState == dSM.mSM.CrouchWalkState){
                dSM.SwitchState(dSM.IncapacitatedState);
            }
        }
        
    }

    public override void FixedUpdateState(DashStateManager dSM){

    }
}
