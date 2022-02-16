using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AerialGrappleGroundedState : AerialBaseState
{
    public override void EnterState(AerialStateManager aSM, AerialBaseState previousState){

        aSM.release = false;
    }

    public override void ExitState(AerialStateManager aSM, AerialBaseState nextState){

    }

    public override void UpdateState(AerialStateManager aSM){
        if(!aSM.isGrounded && aSM.pStats.GravVel < 0){
            aSM.SwitchState(aSM.GrappleAirState);
        }

        if(Vector3.Distance(aSM.transform.position, aSM.hookPoint.transform.position) > aSM.maxGrappleDistance){
            aSM.SwitchState(aSM.GroundedState);
        }

        if((Input.GetKeyDown(KeyCode.E) || Input.GetKeyDown(KeyCode.JoystickButton2)) && !aSM.eHeld){
            aSM.SwitchState(aSM.GroundedState);
        }
        else if((Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.JoystickButton2))){
            aSM.eHeld = false;
        }
    }

    public override void FixedUpdateState(AerialStateManager aSM){
        Debug.DrawRay(aSM.transform.position, (aSM.hookPoint.transform.position - aSM.transform.position)); //Visual of line

        aSM.GravityCalculation(aSM.pStats.PlayerGrav);
    }
}
