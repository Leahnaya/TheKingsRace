using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dDashCooldownState : dDashBaseState
{
    bool cooldown = false; // is cooldown over

    public override void EnterState(dDashStateManager dSM, dDashBaseState previousState){
        cooldown = false; // sets cooldown
        dSM.StartCoroutine(startCoolDown(dSM)); // activates cooldown
    }

    public override void ExitState(dDashStateManager dSM, dDashBaseState nextState){

    }

    public override void UpdateState(dDashStateManager dSM){

        //if after cooldown player is incapacitated still then Incapacitated
        if(cooldown && (dSM.mSM.currentState == dSM.mSM.RagdollState || dSM.mSM.currentState == dSM.mSM.RecoveringState || dSM.mSM.currentState == dSM.mSM.SlideState || dSM.mSM.currentState == dSM.mSM.CrouchState || dSM.mSM.currentState == dSM.mSM.CrouchWalkState)){
            dSM.SwitchState(dSM.IncapacitatedState);
        }

        //if cooldown is over then None
        else if(cooldown){
            dSM.SwitchState(dSM.NoneState);
        }
    }

    public override void FixedUpdateState(dDashStateManager dSM){

    }

    //cooldown function
    private IEnumerator startCoolDown(dDashStateManager dSM){
        dSM.driver.startUICooldown("Dash");
        yield return new WaitForSeconds(dSM.dashItem.cooldownM);
        cooldown = true;
    }
}
