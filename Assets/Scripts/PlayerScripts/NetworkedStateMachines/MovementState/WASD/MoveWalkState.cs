using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWalkState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){
        mSM.GetComponent<Animator>().SetBool("isWalking", true);
        //Debug.Log("Entered Walk");
    }
    
    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){
        mSM.GetComponent<Animator>().SetBool("isWalking", false);
        //Debug.Log("Exited Walk");
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
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.GlidingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState) && !mSM.pStats.IsPaused){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        //actual directional movemnt
        mSM.DirectionalMovement();
    }
}
