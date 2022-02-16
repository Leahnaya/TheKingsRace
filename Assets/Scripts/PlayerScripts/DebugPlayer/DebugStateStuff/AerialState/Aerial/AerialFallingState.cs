using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialFallingState : AerialBaseState
{
    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){
        Debug.Log("Falling State");
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){
        if(aSM.pStats.GravVel > 0){
            aSM.SwitchState(aSM.JumpingState);
        }
        else if(Input.GetButton("Jump") && aSM.pStats.HasGlider && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GlidingState);
        }

        if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }

        if(aSM.isWallRunning && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.WallRunState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);

        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }    

        if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }
}
