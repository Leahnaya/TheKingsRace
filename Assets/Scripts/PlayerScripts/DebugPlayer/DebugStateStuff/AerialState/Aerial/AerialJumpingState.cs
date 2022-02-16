using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialJumpingState : AerialBaseState
{

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){
        Debug.Log("Jumping State");
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){

        if(aSM.pStats.GravVel < 0){
            aSM.SwitchState(aSM.FallingState);
        }

        if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }

        if(aSM.isWallRunning && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.WallRunState);
        }

        if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);

        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }  
    }
}
