using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRunState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM){
        Debug.Log("Run State");
        //Debug.Log(mSM.calculatedCurVel);
    }
    
    public override void UpdateState(MoveStateManager mSM){

        //move to Jog if speed decreases
        if(mSM.calculatedCurVel < mSM.runLimit){
            mSM.SwitchState(mSM.JogState);
        }

        //move to slide if Q or JoystickButton1
        if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && mSM.aSM.currentState != mSM.aSM.FallingState){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        mSM.DirectionalMovement();
    }

    public override void OnCollisionEnter(MoveStateManager mSM){

    }
}
