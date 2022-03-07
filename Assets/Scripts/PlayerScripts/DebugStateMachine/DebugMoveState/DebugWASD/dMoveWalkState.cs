using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveWalkState : dMoveBaseState
{

    //set reference to animator in mSM for each manager
    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){
        //get component is semi expensive--may want to ahve all statemanagers have a reference to animator
        mSM.GetComponent<Animator>().SetBool("isWalking", true);
        Debug.Log("Entered Walk");
    }
    
    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){
        mSM.GetComponent<Animator>().SetBool("isWalking", false);
        Debug.Log("Exited Walk");
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
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState && mSM.aSM.currentState != mSM.aSM.GrappleGroundedState) && !mSM.pStats.IsPaused){
            mSM.SwitchState(mSM.SlideState);
        }
    }

    public override void FixedUpdateState(dMoveStateManager mSM){
        //actual directional movemnt
        mSM.DirectionalMovement();
    }
}
