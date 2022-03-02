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
        
        //if player has nitro
        if(nSM.pStats.HasNitro){
            
            //if ragdolling then incapacitiated
            if(nSM.mSM.currentState == nSM.mSM.RagdollState){
                nSM.SwitchState(nSM.IncapacitatedState);
            }

            //if pressing left shift then nitroing
            else if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton8)) && !nSM.pStats.IsPaused)
            {
                nSM.SwitchState(nSM.NitroingState);
            }

        }
        
    }

    public override void FixedUpdateState(NitroStateManager nSM){

    }
}
