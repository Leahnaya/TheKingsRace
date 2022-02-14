using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIdleState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM){
        Debug.Log("Idle State");
    }
    
    public override void UpdateState(MoveStateManager mSM){

        //Move to Walk State after speed increases
        if(mSM.calculatedCurVel >= mSM.idleLimit){
            mSM.SwitchState(mSM.WalkState);
        }

        //If Q or joystick button1 crouch state
        if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && mSM.aSM.currentState != mSM.aSM.FallingState){
            mSM.SwitchState(mSM.CrouchState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        mSM.DirectionalMovement();
    }

    public override void OnCollisionEnter(MoveStateManager mSM){

    }
}
