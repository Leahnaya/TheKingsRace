using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dNitroCooldownState : dNitroBaseState
{
    bool cooldown = false; // whether or not cooldown has ended

    public override void EnterState(dNitroStateManager nSM, dNitroBaseState previousState){
        cooldown = false; // cooldown hasn't ended

        //start cooldown
        nSM.StartCoroutine(startCoolDown(nSM));
    }

    public override void ExitState(dNitroStateManager nSM, dNitroBaseState nextState){

    }

    public override void UpdateState(dNitroStateManager nSM){
        
        //if off cooldown and incapacitated then incapacitated
        if(cooldown && (nSM.mSM.currentState == nSM.mSM.RagdollState || nSM.mSM.currentState == nSM.mSM.RecoveringState)){
            nSM.SwitchState(nSM.IncapacitatedState);
        }

        //if off cooldown then None
        else if(cooldown){
            nSM.SwitchState(nSM.NoneState);
        }
    }

    public override void FixedUpdateState(dNitroStateManager nSM){

    }

    //cooldown timer
    private IEnumerator startCoolDown(dNitroStateManager nSM){
        //nSM.driver.startUICooldown("Nitro");
        yield return new WaitForSeconds(nSM.nitroItem.cooldownM);
        cooldown = true;
    }
}
