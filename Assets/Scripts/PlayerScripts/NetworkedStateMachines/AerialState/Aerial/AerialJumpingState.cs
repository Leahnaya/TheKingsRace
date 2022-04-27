using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialJumpingState : AerialBaseState
{

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){

        //if grav vel < 0 falling
        if(aSM.pStats.GravVel < 0 || aSM.mSM.currentState == aSM.mSM.RagdollState){
            aSM.SwitchState(aSM.FallingState);
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

    public override void FixedUpdateState(AerialStateManager aSM){

        //default gravity calculations
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);

        //if grapple release then apply grapple release force
        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }  
    }
}
