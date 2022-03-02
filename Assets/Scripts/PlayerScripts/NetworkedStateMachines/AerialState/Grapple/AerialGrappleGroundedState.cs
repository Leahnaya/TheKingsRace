using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGrappleGroundedState : AerialBaseState
{
    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

        aSM.release = false; // release is false when grounded

    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){

        //if E is pressed or ragdolling then grounded
        if(((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) && !aSM.eHeld && !aSM.pStats.IsPaused) || (aSM.mSM.currentState == aSM.mSM.RagdollState)){
            aSM.SwitchState(aSM.GroundedState);
        }
        else if((Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.JoystickButton2)) && aSM.eHeld){
            aSM.eHeld = false;
        }

        //if not grounded and gravVel < 0 then grapple air
        else if(!aSM.isGrounded && aSM.pStats.GravVel < 0){
            aSM.SwitchState(aSM.GrappleAirState);
        }

        //if distance between player and hookpoint is too far then grounded
        else if(Vector3.Distance(aSM.transform.position, aSM.hookPoint.transform.position) > aSM.maxGrappleDistance){
            aSM.SwitchState(aSM.GroundedState);
        }

        
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        Debug.DrawRay(aSM.transform.position, (aSM.hookPoint.transform.position - aSM.transform.position)); //Visual of line

        //Default gravity calculation
        aSM.GravityCalculation(aSM.pStats.PlayerGrav);
    }
}
