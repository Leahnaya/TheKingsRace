using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dAerialGlidingState : dAerialBaseState
{

    public override void EnterState(dAerialStateManager aSM, dAerialBaseState previousState){

        //Modify base traction
        aSM.pStats.CurTraction = 1.0f;
    }

    public override void ExitState(dAerialStateManager aSM, dAerialBaseState nextState){

        //return traction to normal
        aSM.pStats.CurTraction = aSM.pStats.Traction;
    }

    public override void UpdateState(dAerialStateManager aSM){

        //if not holding jump fall
        if(!Input.GetButton("Jump") || (aSM.mSM.currentState == aSM.mSM.RagdollState)){
            aSM.SwitchState(aSM.FallingState);
        }

        //if is grounded then grounded
        else if(aSM.isGrounded){
            aSM.SwitchState(aSM.GroundedState);
        }
        
        //if isWallrunning and in state that allows it wallrun
        else if(aSM.isWallRunning){
            aSM.SwitchState(aSM.WallRunState);
        }

        //if can grapple and in state that allows it grapple
        else if(aSM.CheckGrapple()){
            aSM.SwitchState(aSM.GrappleAirState);
        }
    }

    public override void FixedUpdateState(dAerialStateManager aSM){
        
        //modified gravity calculation to fall slower
        aSM.GravityCalculation(9);

        //if grapple released apply release force
        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }  
    }
}
