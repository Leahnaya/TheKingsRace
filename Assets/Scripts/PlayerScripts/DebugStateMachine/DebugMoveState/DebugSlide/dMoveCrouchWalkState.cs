using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveCrouchWalkState : dMoveBaseState
{
    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){

    }

    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){
        //if next state isn't crouch revert rotation, speed, and height
        if(nextState != mSM.CrouchState){
            mSM.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            mSM.pStats.CurVel = mSM.calculatedCurVel;
            mSM.moveController.height *= 2.0f;
            mSM.moveController.center = new Vector3(0,mSM.moveController.center.y + mSM.moveController.height * .25f,0);
        }
    }
    
    public override void UpdateState(dMoveStateManager mSM){
        if((!Input.GetKey(KeyCode.JoystickButton1) && !Input.GetKey(KeyCode.Q))){
            mSM.SwitchState(mSM.CrouchState);
        }
        else if((Input.GetAxis("Vertical") == 0.0f && Input.GetAxis("Horizontal") == 0.0f)){
            mSM.SwitchState(mSM.CrouchState);
        }

                //If falling stop sliding and go to wasd states
        if(mSM.aSM.currentState == mSM.aSM.FallingState){
             //Determine which state to go into based on player speed
            if(mSM.calculatedCurVel < mSM.walkLimit){
                mSM.SwitchState(mSM.WalkState);
            }
            else if(mSM.calculatedCurVel < mSM.runLimit){
                mSM.SwitchState(mSM.JogState);
            }
            else{
                mSM.SwitchState(mSM.RunState);
            }
        }
    }

    public override void FixedUpdateState(dMoveStateManager mSM){

        mSM.CrouchMovement();
    }

}
