using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveWalkState : dMoveBaseState
{
    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){

    }
    
    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

    }

    public override void UpdateState(dMoveStateManager mSM){

        //move to Jog if speed increases
        if(mSM.calculatedCurVel >= mSM.jogLimit){
            mSM.SwitchState(mSM.JogState);
        }
        //move to Idle if speed decreases
        else if(mSM.calculatedCurVel < mSM.idleLimit){
            mSM.SwitchState(mSM.IdleState);
        }

        //move to slide if Q or JoystickButton1
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState && mSM.aSM.currentState != mSM.aSM.GrappleGroundedState)){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(dMoveStateManager mSM){
        //actual directional movemnt
        mSM.DirectionalMovement();
    }
}
