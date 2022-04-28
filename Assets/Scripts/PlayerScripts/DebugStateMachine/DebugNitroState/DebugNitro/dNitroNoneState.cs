using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dNitroNoneState : dNitroBaseState
{
    public override void EnterState(dNitroStateManager nSM, dNitroBaseState previousState){

    }

    public override void ExitState(dNitroStateManager nSM, dNitroBaseState nextState){

    }

    public override void UpdateState(dNitroStateManager nSM){
        
        //if player has nitro
        if(nSM.pStats.HasNitro){
            
            //if ragdolling then incapacitiated
            if(nSM.mSM.currentState == nSM.mSM.RagdollState){
                nSM.SwitchState(nSM.IncapacitatedState);
            }

            //if pressing left shift then nitroing
            else if ((Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.JoystickButton4)) && !nSM.pStats.IsPaused)
            {
                nSM.SwitchState(nSM.NitroingState);
            }

        }
        
    }

    public override void FixedUpdateState(dNitroStateManager nSM){

    }
}
