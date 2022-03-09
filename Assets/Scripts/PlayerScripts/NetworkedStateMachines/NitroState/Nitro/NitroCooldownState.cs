using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroCooldownState : NitroBaseState
{
    bool cooldown = false; // whether or not cooldown has ended

    public override void EnterState(NitroStateManager nSM, NitroBaseState previousState){
        cooldown = false; // cooldown hasn't ended

        //start cooldown
        nSM.StartCoroutine(startCoolDown(nSM));
    }

    public override void ExitState(NitroStateManager nSM, NitroBaseState nextState){

    }

    public override void UpdateState(NitroStateManager nSM){
        
        //if off cooldown and incapacitated then incapacitated
        if(cooldown && (nSM.mSM.currentState == nSM.mSM.RagdollState || nSM.mSM.currentState == nSM.mSM.RecoveringState)){
            nSM.SwitchState(nSM.IncapacitatedState);
        }

        //if off cooldown then None
        else if(cooldown){
            nSM.SwitchState(nSM.NoneState);
        }
    }

    public override void FixedUpdateState(NitroStateManager nSM){

    }

    //cooldown timer
    private IEnumerator startCoolDown(NitroStateManager nSM){
        nSM.driver.startUICooldown("Nitro");
        yield return new WaitForSeconds(nSM.nitroItem.cooldownM);
        cooldown = true;
    }
}
