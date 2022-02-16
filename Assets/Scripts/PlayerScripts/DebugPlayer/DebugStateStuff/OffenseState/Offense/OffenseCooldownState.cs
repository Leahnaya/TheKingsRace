using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenseCooldownState : OffenseBaseState
{
    bool cooldown = false;

    public override void EnterState(OffenseStateManager oSM, OffenseBaseState previousState){
        cooldown = false;

        oSM.StartCoroutine(kickCooldown());
    }   

    public override void ExitState(OffenseStateManager oSM, OffenseBaseState nextState){

    }

    public override void UpdateState(OffenseStateManager oSM){
        if(cooldown && (oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState || oSM.aSM.currentState == oSM.aSM.GrappleGroundedState)){
            oSM.SwitchState(oSM.IncapacitatedState);
        }
        else if(cooldown){
            oSM.SwitchState(oSM.NoneState);
        }
    }

    public override void FixedUpdateState(OffenseStateManager oSM){

    }

    private IEnumerator kickCooldown(){
        yield return new WaitForSeconds(.5f);
        cooldown = true;
    }
}
