using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NitroNoneState : NitroBaseState
{
    public override void EnterState(NitroStateManager nSM, NitroBaseState previousState){

    }

    public override void ExitState(NitroStateManager nSM, NitroBaseState nextState){

    }

    public override void UpdateState(NitroStateManager nSM){
        if(nSM.pStats.HasNitro){

            if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton8)))
            {
                nSM.SwitchState(nSM.NitroingState);
            }

            if(nSM.mSM.currentState == nSM.mSM.RagdollState){
                nSM.SwitchState(nSM.IncapacitatedState);
            }
        }
        
    }

    public override void FixedUpdateState(NitroStateManager nSM){

    }
}
