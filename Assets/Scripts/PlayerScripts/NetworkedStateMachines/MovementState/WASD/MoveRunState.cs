using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRunState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){

    }
    
    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){

    }

    public override void UpdateState(MoveStateManager mSM){

        //move to Jog if speed decreases
        if(mSM.calculatedCurVel < mSM.runLimit){
            mSM.SwitchState(mSM.JogState);
        }

        //move to slide if Q or JoystickButton1
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState && mSM.aSM.currentState != mSM.aSM.GrappleGroundedState) && !mSM.pStats.IsPaused){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        //actual direction movement
        mSM.DirectionalMovement();
    }
}
