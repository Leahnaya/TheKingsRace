using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dNitroNitroingState : dNitroBaseState
{

    private float tempTimer; // temporary timer
    private float actualMaxVel; // actual max velocity
    private float actualAcc; // actual acceleration

    public override void EnterState(dNitroStateManager nSM, dNitroBaseState previousState){
        actualMaxVel = nSM.pStats.MaxVel; // saves previous max velocity
        actualAcc = nSM.pStats.Acc; // saves previous max acc
        nSM.pStats.Acc += nSM.nitroAccBoost; // increases acceleration
        nSM.pStats.MaxVel += nSM.nitroVelBoost; // increases max velocity

        tempTimer = 5; // how long we will be sped up
    }

    public override void ExitState(dNitroStateManager nSM, dNitroBaseState nextState){
        nSM.pStats.Acc = actualAcc; // reset acceleration
        nSM.pStats.MaxVel = actualMaxVel; // reset maximum velocity
    }

    public override void UpdateState(dNitroStateManager nSM){

        //if still nitroing decrease timer
        if(tempTimer > 0){            
            tempTimer -= .02f;
        }

        //otherwise cooldown
        else{
            nSM.SwitchState(nSM.CooldownState);
        }

        //if ragdolling then cooldown
        if(nSM.mSM.currentState == nSM.mSM.RagdollState){
            nSM.SwitchState(nSM.CooldownState);
        }
    }

    public override void FixedUpdateState(dNitroStateManager nSM){

    }
}
