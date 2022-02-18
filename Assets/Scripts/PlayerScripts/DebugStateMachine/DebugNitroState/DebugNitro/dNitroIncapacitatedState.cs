using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dNitroIncapacitatedState : dNitroBaseState
{
    public override void EnterState(dNitroStateManager nSM, dNitroBaseState previousState){

    }

    public override void ExitState(dNitroStateManager nSM, dNitroBaseState nextState){

    }

    public override void UpdateState(dNitroStateManager nSM){

        //if no longer incapacitated then None
        if(nSM.mSM.currentState != nSM.mSM.RagdollState && nSM.mSM.currentState != nSM.mSM.RecoveringState){
            nSM.SwitchState(nSM.NoneState);
        }
    }

    public override void FixedUpdateState(dNitroStateManager nSM){

    }
}
