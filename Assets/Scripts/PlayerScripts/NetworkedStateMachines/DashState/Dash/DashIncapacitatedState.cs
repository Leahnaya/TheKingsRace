using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashIncapacitatedState : DashBaseState
{
    public override void EnterState(DashStateManager dSM, DashBaseState previousState){

    }

    public override void ExitState(DashStateManager dSM, DashBaseState nextState){

    }

    public override void UpdateState(DashStateManager dSM){

        //if no longer incapacitated then None
        if(dSM.mSM.currentState != dSM.mSM.RagdollState && dSM.mSM.currentState != dSM.mSM.RecoveringState && dSM.mSM.currentState != dSM.mSM.SlideState && dSM.mSM.currentState != dSM.mSM.CrouchState && dSM.mSM.currentState != dSM.mSM.CrouchWalkState){
            dSM.SwitchState(dSM.NoneState);
        }
    }

    public override void FixedUpdateState(DashStateManager dSM){

    }
}
