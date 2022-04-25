using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRunState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){
        mSM.GetComponent<Animator>().SetBool("isRunning", true);
        //Debug.Log("Entered Run");
    }
    
    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){
        mSM.GetComponent<Animator>().SetBool("isRunning", false);
        //Debug.Log("Exit Run");
    }

    public override void UpdateState(MoveStateManager mSM){

        //move to Jog if speed decreases
        if(mSM.calculatedCurVel < mSM.runLimit){
            mSM.SwitchState(mSM.JogState);
        }

        //move to slide if Q or JoystickButton1
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKeyDown(GameManager.GM.bindableActions["slideKey"])) && (mSM.aSM.currentState != mSM.aSM.GrappleAirState && mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.GlidingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState) && !mSM.pStats.IsPaused){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        //actual direction movement
        mSM.DirectionalMovement();
    }
}
