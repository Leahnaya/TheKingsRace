using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialWallRunState : AerialBaseState
{
    
    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

        aSM.pStats.GravVel = 0;

    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){
        if(Input.GetButton("Jump")){
            aSM.SwitchState(aSM.JumpingState);
        }
        else if(!aSM.isWallRunning){
            aSM.SwitchState(aSM.FallingState);
        }

        if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        aSM.GravityCalculation(2);
    }
}
