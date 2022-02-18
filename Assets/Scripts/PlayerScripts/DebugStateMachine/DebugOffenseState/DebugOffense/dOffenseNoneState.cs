using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dOffenseNoneState : dOffenseBaseState
{
    public override void EnterState(dOffenseStateManager oSM, dOffenseBaseState previousState){

    }

    public override void ExitState(dOffenseStateManager oSM, dOffenseBaseState nextState){

    }

    public override void UpdateState(dOffenseStateManager oSM){
        
        //if incapacitated then incapacitated
        if((oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState || oSM.aSM.currentState == oSM.aSM.GrappleGroundedState)){
            oSM.SwitchState(oSM.IncapacitatedState);
        }

        //if grounded then grounded kick states
        else if(oSM.aSM.currentState == oSM.aSM.GroundedState && (Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0)){

            //if power is above 300 then punch
            if(oSM.pStats.KickPow > 300){
                oSM.SwitchState(oSM.PunchState);
            }

            //otherwise kick
            else{
                oSM.SwitchState(oSM.KickState);
            }

        }

        //if in the air
        else if((Input.GetKeyDown(KeyCode.F) || Input.GetAxis("Kick") != 0)){

            //if power is above 300 air punch
            if(oSM.pStats.KickPow > 300){
                oSM.SwitchState(oSM.AirPunchState);
            }

            //otherwise air kick
            else{
                oSM.SwitchState(oSM.AirKickState);
            }
        }
    }

    public override void FixedUpdateState(dOffenseStateManager oSM){

    }
}
