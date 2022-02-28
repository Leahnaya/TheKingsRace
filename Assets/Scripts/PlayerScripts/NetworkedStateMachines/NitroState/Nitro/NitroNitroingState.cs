using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroNitroingState : NitroBaseState
{

    private float tempTimer; // temporary timer
    private float actualMaxVel; // actual max velocity

    public override void EnterState(NitroStateManager nSM, NitroBaseState previousState){
        actualMaxVel = nSM.pStats.MaxVel; // saves previous max velocity

        nSM.pStats.CurAcc += nSM.nitroAccBoost; // increases acceleration
        nSM.pStats.MaxVel += nSM.nitroVelBoost; // increases max velocity

        tempTimer = 5; // how long we will be sped up
    }

    public override void ExitState(NitroStateManager nSM, NitroBaseState nextState){
        nSM.pStats.CurAcc = nSM.pStats.Acc; // reset acceleration
        nSM.pStats.MaxVel = actualMaxVel; // reset maximum velocity
    }

    public override void UpdateState(NitroStateManager nSM){

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

    public override void FixedUpdateState(NitroStateManager nSM){

    }
}
