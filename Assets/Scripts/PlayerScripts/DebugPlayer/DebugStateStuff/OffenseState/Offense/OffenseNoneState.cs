using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenseNoneState : OffenseBaseState
{
    public override void EnterState(OffenseStateManager oSM, OffenseBaseState previousState){

    }

    public override void ExitState(OffenseStateManager oSM, OffenseBaseState nextState){

    }

    public override void UpdateState(OffenseStateManager oSM){

        if((oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState || oSM.aSM.currentState == oSM.aSM.GrappleGroundedState)){
            oSM.SwitchState(oSM.IncapacitatedState);
        }
        else if(oSM.aSM.currentState == oSM.aSM.GroundedState && (Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0)){
            if(oSM.pStats.KickPow > 300){
                oSM.SwitchState(oSM.PunchState);
            }
            else{
                oSM.SwitchState(oSM.KickState);
            }
        }
        else if((Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0)){
            if(oSM.pStats.KickPow > 300){
                oSM.SwitchState(oSM.AirPunchState);
            }
            else{
                oSM.SwitchState(oSM.AirKickState);
            }
        }
    }

    public override void FixedUpdateState(OffenseStateManager oSM){

    }
}
