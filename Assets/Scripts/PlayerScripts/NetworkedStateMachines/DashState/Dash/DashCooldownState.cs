using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCooldownState : DashBaseState
{
    bool cooldown = false;

    public override void EnterState(DashStateManager dSM, DashBaseState previousState){
        cooldown = false;
        dSM.StartCoroutine(startCoolDown(dSM));
    }

    public override void ExitState(DashStateManager dSM, DashBaseState nextState){

    }

    public override void UpdateState(DashStateManager dSM){
        if(cooldown && (dSM.mSM.currentState == dSM.mSM.RagdollState || dSM.mSM.currentState == dSM.mSM.RecoveringState || dSM.mSM.currentState == dSM.mSM.SlideState || dSM.mSM.currentState == dSM.mSM.CrouchState || dSM.mSM.currentState == dSM.mSM.CrouchWalkState)){
            dSM.SwitchState(dSM.IncapacitatedState);
        }
        else if(cooldown){
            dSM.SwitchState(dSM.NoneState);
        }
    }

    public override void FixedUpdateState(DashStateManager dSM){

    }

    private IEnumerator startCoolDown(DashStateManager dSM){
        //dSM.driver.startUICooldown(dashItem.name);
        yield return new WaitForSeconds(dSM.dashItem.cooldownM);
        cooldown = true;
    }
}
