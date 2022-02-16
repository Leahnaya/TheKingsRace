using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroCooldownState : NitroBaseState
{
    bool cooldown = false;

    public override void EnterState(NitroStateManager nSM, NitroBaseState previousState){
        cooldown = false;
        nSM.StartCoroutine(startCoolDown(nSM));
    }

    public override void ExitState(NitroStateManager nSM, NitroBaseState nextState){

    }

    public override void UpdateState(NitroStateManager nSM){
        if(cooldown && (nSM.mSM.currentState == nSM.mSM.RagdollState || nSM.mSM.currentState == nSM.mSM.RecoveringState)){
            nSM.SwitchState(nSM.IncapacitatedState);
        }
        else if(cooldown){
            nSM.SwitchState(nSM.NoneState);
        }
    }

    public override void FixedUpdateState(NitroStateManager nSM){

    }

    private IEnumerator startCoolDown(NitroStateManager nSM){
        //nSM.driver.startUICooldown("Nitro");
        yield return new WaitForSeconds(nSM.nitroItem.cooldownM);
        cooldown = true;
    }
}
