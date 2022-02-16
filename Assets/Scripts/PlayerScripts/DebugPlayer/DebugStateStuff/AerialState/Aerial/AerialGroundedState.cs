using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGroundedState : AerialBaseState
{
    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){
        Debug.Log("Grounded State");

        aSM.release = false;
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){
        if(aSM.pStats.GravVel < 0){
            aSM.SwitchState(aSM.FallingState);
        }
        else if(aSM.pStats.GravVel > 0){
            aSM.SwitchState(aSM.JumpingState);
        }

        if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleGroundedState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);
    }
}
