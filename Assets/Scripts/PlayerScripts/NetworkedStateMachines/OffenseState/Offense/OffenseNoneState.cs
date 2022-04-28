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
        
        //if incapacitated then incapacitated
        if((oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState)){
            oSM.SwitchState(oSM.IncapacitatedState);
        }

        //if grounded then grounded kick states
        else if(oSM.aSM.currentState == oSM.aSM.GroundedState && (Input.GetKeyDown(GameManager.GM.bindableActions["kickKey"]) || Input.GetKeyDown(KeyCode.JoystickButton2)) && !oSM.pStats.IsPaused){
            //Regular Kick
            oSM.SwitchState(oSM.KickState);
        }

        //if in the air
        else if((Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.JoystickButton2) && !oSM.pStats.IsPaused)){
            oSM.SwitchState(oSM.AirKickState);
        }
    }

    public override void FixedUpdateState(OffenseStateManager oSM){

    }
}
