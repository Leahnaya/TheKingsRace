using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveJogState : dMoveBaseState
{
    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){
        mSM.GetComponent<Animator>().SetBool("isJogging", true);
        //Debug.Log("Entered Jog");
    }

    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){
        mSM.GetComponent<Animator>().SetBool("isJogging", false);
        //Debug.Log("Exited Jog");
    }

    public override void UpdateState(dMoveStateManager mSM){

        //move to run state if speed increases
        if(mSM.calculatedCurVel >= mSM.runLimit){
            mSM.SwitchState(mSM.RunState);
        }
        //move to walk if speed decreases
        else if(mSM.calculatedCurVel < mSM.walkLimit){
            mSM.SwitchState(mSM.WalkState);
        }

        //move to slide if Q or JoystickButton1
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState && mSM.aSM.currentState != mSM.aSM.GrappleGroundedState) && !mSM.pStats.IsPaused){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(dMoveStateManager mSM){

        //actual directional movment
        mSM.DirectionalMovement();
    }
}
