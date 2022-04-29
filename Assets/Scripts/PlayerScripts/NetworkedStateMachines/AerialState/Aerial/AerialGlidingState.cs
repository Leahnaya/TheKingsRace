using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGlidingState : AerialBaseState
{

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

        //Modify base traction
        aSM.pStats.CurTraction = 1.0f;
        aSM.pStats.GravVel = -4;
        aSM.GetComponent<Animator>().SetBool("isGliding", true);
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

        //return traction to normal
        aSM.pStats.CurTraction = aSM.pStats.Traction;
        aSM.GetComponent<Animator>().SetBool("isGliding", false);
    }

    public override void UpdateState(AerialStateManager aSM){

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

    public override void FixedUpdateState(AerialStateManager aSM){
        
        //modified gravity calculation to fall slower
        aSM.GravityCalculation(9);

        //if grapple released apply release force
        if(aSM.pStats.HasGrapple){
            aSM.GrappleReleaseForce();
        }  
    }
}
