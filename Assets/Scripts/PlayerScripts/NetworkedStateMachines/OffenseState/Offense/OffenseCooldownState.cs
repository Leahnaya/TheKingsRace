using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffenseCooldownState : OffenseBaseState
{
    bool cooldown = false; // whether or not cooldown is over

    public override void EnterState(OffenseStateManager oSM, OffenseBaseState previousState){
        cooldown = false; // cooldown isn't over
        //start cooldown
        oSM.StartCoroutine(kickCooldown());
    }   

    public override void ExitState(OffenseStateManager oSM, OffenseBaseState nextState){

    }

    public override void UpdateState(OffenseStateManager oSM){

        //if cooldown over and incapacitated then incapacitated
        if(cooldown && (oSM.mSM.currentState == oSM.mSM.RagdollState || oSM.mSM.currentState == oSM.mSM.SlideState || oSM.mSM.currentState == oSM.mSM.CrouchState || oSM.mSM.currentState == oSM.mSM.CrouchWalkState) || (oSM.aSM.currentState == oSM.aSM.WallRunState || oSM.aSM.currentState == oSM.aSM.WallIdleState || oSM.aSM.currentState == oSM.aSM.GrappleAirState)){
            oSM.SwitchState(oSM.IncapacitatedState);
        }

        //if cooldown over then None
        else if(cooldown){
            oSM.SwitchState(oSM.NoneState);
        }
    }

    public override void FixedUpdateState(OffenseStateManager oSM){

    }

    //kick cooldown
    private IEnumerator kickCooldown(){
        yield return new WaitForSeconds(.5f);
        cooldown = true;
    }
}
