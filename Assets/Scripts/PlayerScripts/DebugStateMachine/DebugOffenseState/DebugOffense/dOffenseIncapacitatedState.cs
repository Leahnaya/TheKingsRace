using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dOffenseIncapacitatedState : dOffenseBaseState
{
    public override void EnterState(dOffenseStateManager oSM, dOffenseBaseState previousState){

    }

    public override void ExitState(dOffenseStateManager oSM, dOffenseBaseState nextState){

    }

    public override void UpdateState(dOffenseStateManager oSM){

        //if no longer incapacitated then None
        if((oSM.mSM.currentState != oSM.mSM.RagdollState && oSM.mSM.currentState != oSM.mSM.SlideState && oSM.mSM.currentState != oSM.mSM.CrouchState && oSM.mSM.currentState != oSM.mSM.CrouchWalkState) && (oSM.aSM.currentState != oSM.aSM.WallRunState && oSM.aSM.currentState != oSM.aSM.WallIdleState && oSM.aSM.currentState != oSM.aSM.GrappleAirState && oSM.aSM.currentState != oSM.aSM.GrappleGroundedState)){
            oSM.SwitchState(oSM.NoneState);
        }
    }

    public override void FixedUpdateState(dOffenseStateManager oSM){

    }
}
