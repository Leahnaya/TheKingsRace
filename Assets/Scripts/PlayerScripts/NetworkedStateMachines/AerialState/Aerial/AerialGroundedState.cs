using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGroundedState : AerialBaseState
{
    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

        //release is false if grounded
        aSM.release = false;
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){

        //if grav vel < 0 then falling
        if(aSM.pStats.GravVel < 0 || (aSM.pStats.GravVel > 0 && (aSM.mSM.currentState == aSM.mSM.SlideState || aSM.mSM.currentState == aSM.mSM.CrouchState || aSM.mSM.currentState == aSM.mSM.CrouchWalkState || aSM.mSM.currentState == aSM.mSM.RagdollState || aSM.mSM.currentState == aSM.mSM.RecoveringState))){
            aSM.SwitchState(aSM.FallingState);
        }
        //if grav vel > 0 then jumping
        else if(aSM.pStats.GravVel > 0 && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.CrouchState && aSM.mSM.currentState != aSM.mSM.CrouchWalkState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.JumpingState);
        }

        //can grapple and in state that allows grapple
        else if(aSM.CheckGrapple() && (aSM.mSM.currentState != aSM.mSM.SlideState && aSM.mSM.currentState != aSM.mSM.CrouchState && aSM.mSM.currentState != aSM.mSM.CrouchWalkState && aSM.mSM.currentState != aSM.mSM.RagdollState && aSM.mSM.currentState != aSM.mSM.RecoveringState)){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){

        //base gravity calculations
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);
    }
}
