using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCooldownState : DashBaseState
{
    bool cooldown = false; // is cooldown over

    public override void EnterState(DashStateManager dSM, DashBaseState previousState){
        cooldown = false; // sets cooldown
        dSM.StartCoroutine(startCoolDown(dSM)); // activates cooldown
    }

    public override void ExitState(DashStateManager dSM, DashBaseState nextState){

    }

    public override void UpdateState(DashStateManager dSM){

        //if after cooldown player is incapacitated still then Incapacitated
        if(cooldown && (dSM.mSM.currentState == dSM.mSM.RagdollState || dSM.mSM.currentState == dSM.mSM.RecoveringState || dSM.mSM.currentState == dSM.mSM.SlideState || dSM.mSM.currentState == dSM.mSM.CrouchState || dSM.mSM.currentState == dSM.mSM.CrouchWalkState)){
            dSM.SwitchState(dSM.IncapacitatedState);
        }

        //if cooldown is over then None
        else if(cooldown){
            dSM.SwitchState(dSM.NoneState);
        }
    }

    public override void FixedUpdateState(DashStateManager dSM){

    }

    //cooldown function
    private IEnumerator startCoolDown(DashStateManager dSM){
        //dSM.driver.startUICooldown(dashItem.name);
        yield return new WaitForSeconds(dSM.dashItem.cooldownM);
        cooldown = true;
    }
}
