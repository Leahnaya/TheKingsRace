using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroIncapacitatedState : NitroBaseState
{
    public override void EnterState(NitroStateManager nSM, NitroBaseState previousState){

    }

    public override void ExitState(NitroStateManager nSM, NitroBaseState nextState){

    }

    public override void UpdateState(NitroStateManager nSM){
        if(nSM.mSM.currentState != nSM.mSM.RagdollState && nSM.mSM.currentState != nSM.mSM.RecoveringState){
            nSM.SwitchState(nSM.NoneState);
        }
    }

    public override void FixedUpdateState(NitroStateManager nSM){

    }
}
