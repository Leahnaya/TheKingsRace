using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGlidingState : AerialBaseState
{
    float tempTraction; // temp traction to store the actual player traction

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

        //Modify base traction
        tempTraction = aSM.pStats.Traction;
        aSM.pStats.Traction = 1.0f;
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

        //return traction to normal
        aSM.pStats.Traction = tempTraction;
    }

    public override void UpdateState(AerialStateManager aSM){

        //if not holding jump fall
        if(!Input.GetButton("Jump")){
            aSM.SwitchState(aSM.FallingState);
        }

        //if is grounded then grounded
        if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }
        
        //if isWallrunning and in state that allows it wallrun
        if(aSM.isWallRunning && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.WallRunState);
        }

        //if can grapple and in state that allows it grapple
        if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        
        //modified gravity calculation to fall slower
        aSM.GravityCalculation(9);

        //if grapple released apply release force
        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }  
    }
}