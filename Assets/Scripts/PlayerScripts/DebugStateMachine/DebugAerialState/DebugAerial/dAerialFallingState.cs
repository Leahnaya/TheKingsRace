using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dAerialFallingState : dAerialBaseState
{
    bool shouldGlide;
    public override void EnterState(dAerialStateManager aSM, dAerialBaseState previousState){
        if(previousState == aSM.JumpingState && aSM.jumpHeld){
            shouldGlide = true;
        }
        else{
            shouldGlide = false;
        }
    }

    public override void ExitState(dAerialStateManager aSM, dAerialBaseState nextState){

    }   

    public override void UpdateState(dAerialStateManager aSM){

        //if Grav Vel > 0 then jumping
        if((Input.GetButton("Jump") && aSM.jumpPressed) && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.CrouchState && aSM.mSM.currentState != aSM.mSM.CrouchWalkState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            Debug.Log("Going to Jump From Fall");
            aSM.SwitchState(aSM.JumpingState);
             
        }

        //if jump has been pressed and has glider and is in a state that allows it glide
        else if((Input.GetButton("Jump") && (shouldGlide || (aSM.curJumpNum == aSM.pStats.JumpNum))) && aSM.pStats.HasGlider && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.CrouchState && aSM.mSM.currentState != aSM.mSM.CrouchWalkState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState) && !aSM.pStats.IsPaused){
            Debug.Log("Going to Glide From Fall");
            aSM.SwitchState(aSM.GlidingState);
        }

        //if is grounded then grounded
        else if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }

        //if is wallrunning ands is in a state that allows it wallrun
        else if(aSM.isWallRunning && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.CrouchState && aSM.mSM.currentState != aSM.mSM.CrouchWalkState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.WallRunState);
        }

        //if grapple is possible and in state that allows it grapple air
        else if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.CrouchState && aSM.mSM.currentState != aSM.mSM.CrouchWalkState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(dAerialStateManager aSM){
        
        //Default gravity calculation
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);

        //if grapple released apply release force
        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }    
    }
}
