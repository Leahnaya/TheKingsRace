using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveJogState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){
        mSM.GetComponent<Animator>().SetBool("isJogging", true);
        //Debug.Log("Entered Jog");
    }

    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){
        mSM.GetComponent<Animator>().SetBool("isJogging", false);
        //Debug.Log("Exited Jog");
    }

    public override void UpdateState(MoveStateManager mSM){

        //move to run state if speed increases
        if(mSM.calculatedCurVel >= mSM.runLimit){
            mSM.SwitchState(mSM.RunState);
        }
        //move to walk if speed decreases
        else if(mSM.calculatedCurVel < mSM.walkLimit){
            mSM.SwitchState(mSM.WalkState);
        }

        //move to slide if Q or JoystickButton1
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKeyDown(GameManager.GM.bindableActions["slideKey"])) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.GlidingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState) && !mSM.pStats.IsPaused){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){

        //actual directional movment
        mSM.DirectionalMovement();
    }
}
