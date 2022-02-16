using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroNitroingState : NitroBaseState
{

    private float tempTimer;
    private float actualMaxVel;
    private float actualAcc;

    public override void EnterState(NitroStateManager nSM, NitroBaseState previousState){
        actualMaxVel = nSM.pStats.MaxVel;
        actualAcc = nSM.pStats.Acc;
        nSM.pStats.Acc += nSM.nitroAccBoost;
        nSM.pStats.MaxVel += nSM.nitroVelBoost;

        tempTimer = 5;
    }

    public override void ExitState(NitroStateManager nSM, NitroBaseState nextState){
        nSM.pStats.Acc = actualAcc;
        nSM.pStats.MaxVel = actualMaxVel;
    }

    public override void UpdateState(NitroStateManager nSM){
        if(tempTimer > 0){            
            tempTimer -= .02f;
        }
        else{
            nSM.SwitchState(nSM.CooldownState);
        }

        if(nSM.mSM.currentState == nSM.mSM.RagdollState){
            nSM.SwitchState(nSM.CooldownState);
        }
    }

    public override void FixedUpdateState(NitroStateManager nSM){

    }
}
