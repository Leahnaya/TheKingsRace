using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialWallRunState : AerialBaseState
{
    bool spaceHeld = true;

    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

        aSM.pStats.GravVel = 0; // on entering reset grav vel
        spaceHeld = true; // prevent accidental wall jumping

    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){

        //if not wallrunning or are ragdolling then falling
        if(!aSM.isWallRunning || (aSM.mSM.currentState == aSM.mSM.RagdollState)){
            aSM.SwitchState(aSM.FallingState);
        }        

        //if space is pressed then jumping
        else if(Input.GetButton("Jump") && !spaceHeld){
            aSM.SwitchState(aSM.JumpingState);
        }
        else if(!Input.GetButton("Jump") && spaceHeld){
            spaceHeld = false;
        }

        //if able to grapple then grapple
        else if(aSM.CheckGrapple()){
            aSM.SwitchState(aSM.GrappleAirState);
        }

    }

    public override void FixedUpdateState(AerialStateManager aSM){

        //Modified gravity calculation for wallrun
        aSM.GravityCalculation(2);

    }
}
