using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGlidingState : AerialBaseState
{
    float tempTraction;

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){
        Debug.Log("Gliding State");

        tempTraction = aSM.pStats.Traction;
        aSM.pStats.Traction = 1.0f;
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){
        aSM.pStats.Traction = tempTraction;
    }

    public override void UpdateState(AerialStateManager aSM){
        if(!Input.GetButton("Jump")){
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
        aSM.GravityCalculation(9);

        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }  
    }
}
