using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dDashIncapacitatedState : dDashBaseState
{
    public override void EnterState(dDashStateManager dSM, dDashBaseState previousState){

    }

    public override void ExitState(dDashStateManager dSM, dDashBaseState nextState){

    }

    public override void UpdateState(dDashStateManager dSM){

        //if no longer incapacitated then None
        if(dSM.mSM.currentState != dSM.mSM.RagdollState && dSM.mSM.currentState != dSM.mSM.RecoveringState && dSM.mSM.currentState != dSM.mSM.SlideState && dSM.mSM.currentState != dSM.mSM.CrouchState && dSM.mSM.currentState != dSM.mSM.CrouchWalkState){
            dSM.SwitchState(dSM.NoneState);
        }
    }

    public override void FixedUpdateState(dDashStateManager dSM){

    }
}
