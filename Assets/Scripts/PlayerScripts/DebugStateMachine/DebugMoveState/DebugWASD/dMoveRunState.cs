using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveRunState : dMoveBaseState
{
    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){
        mSM.GetComponent<Animator>().SetBool("isRunning", true);
        Debug.Log("Entered Run");
    }
    
    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){
        mSM.GetComponent<Animator>().SetBool("isRunning", false);
        Debug.Log("Exit Run");
    }

    public override void UpdateState(dMoveStateManager mSM){

        //move to Jog if speed decreases
        if(mSM.calculatedCurVel < mSM.runLimit){
            mSM.SwitchState(mSM.JogState);
        }

        //move to slide if Q or JoystickButton1
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState && mSM.aSM.currentState != mSM.aSM.GrappleGroundedState) && !mSM.pStats.IsPaused){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(dMoveStateManager mSM){
        //actual direction movement
        mSM.DirectionalMovement();
    }
}
