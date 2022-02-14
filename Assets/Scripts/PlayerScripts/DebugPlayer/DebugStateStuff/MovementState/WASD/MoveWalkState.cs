using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWalkState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM){
        Debug.Log("Walk State");
        //Debug.Log(mSM.calculatedCurVel);
    }
    
    public override void UpdateState(MoveStateManager mSM){

        //move to Jog if speed increases
        if(mSM.calculatedCurVel >= mSM.jogLimit){
            mSM.SwitchState(mSM.JogState);
        }
        //move to Idle if speed decreases
        else if(mSM.calculatedCurVel < mSM.idleLimit){
            mSM.SwitchState(mSM.IdleState);
        }

        //move to slide if Q or JoystickButton1
        if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q))){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        mSM.DirectionalMovement();
    }

    public override void OnCollisionEnter(MoveStateManager mSM){

    }
}
