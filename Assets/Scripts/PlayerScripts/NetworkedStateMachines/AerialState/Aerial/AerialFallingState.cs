using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialFallingState : AerialBaseState
{
    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){

        //if Grav Vel > 0 then jumping
        if(aSM.pStats.GravVel > 0){
            aSM.SwitchState(aSM.JumpingState);
        }
        //if jump has been pressed and has glider and is in a state that allows it glide
        else if(Input.GetButton("Jump") && aSM.pStats.HasGlider && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GlidingState);
        }

        //if is grounded then grounded
        if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }

        //if is wallrunning ands is in a state that allows it wallrun
        if(aSM.isWallRunning && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.WallRunState);
        }

        //if grapple is possible and in state that allows it grapple air
        if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        
        //Default gravity calculation
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);

        //if grapple released apply release force
        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }    
    }
}
