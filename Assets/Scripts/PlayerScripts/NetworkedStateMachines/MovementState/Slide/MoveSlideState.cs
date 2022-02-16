using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSlideState : MoveBaseState
{
    //Slide Variables
    float originalTraction; // Traction before slide started
    RaycastHit slideRay; // slide raycast

    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){

        //Initialize Important Stats On state enter
        mSM.pStats.CurVel = 0;
        originalTraction = mSM.pStats.Traction;
        mSM.gameObject.transform.eulerAngles = new Vector3(mSM.transform.eulerAngles.x - 90, mSM.transform.eulerAngles.y, mSM.transform.eulerAngles.z);
        mSM.moveController.height *= .5f;
        mSM.pStats.Traction = 0.01f;
    }
    
    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){
        if(nextState != mSM.CrouchState){
            mSM.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            mSM.pStats.CurVel = mSM.calculatedCurVel;
            mSM.pStats.Traction = originalTraction;
            mSM.moveController.height *= 2.0f;
        }
        else{
            mSM.pStats.Traction = originalTraction;
        }
    }

    public override void UpdateState(MoveStateManager mSM){

        //if player comes to a stop while sliding they crouch
        if(mSM.calculatedCurVel < mSM.idleLimit){
            mSM.SwitchState(mSM.CrouchState);
        }
        
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        mSM.transform.Rotate(Vector3.forward * -mSM.sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));
        mSM.pStats.Traction += .004f;
        
        ///////ONCE WE HAVE IT SO SLIDE DOESNT ROTATE PLAYER MOVE THIS TO UPDATE
        if((!Input.GetKey(KeyCode.JoystickButton1) && !Input.GetKey(KeyCode.Q))){
            if ((Physics.Raycast(mSM.gameObject.transform.position, mSM.slideUp, out slideRay, 5f) == false)){

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
            else{
                Debug.Log("Object above you");
            }

        }

        /*
        //If falling stop sliding and go to wasd states
        if(mSM.aSM.currentState == mSM.aSM.FallingState){
             //Determine which state to go into based on player speed
                if(mSM.calculatedCurVel < mSM.walkLimit){
                    SlideToMoveState(mSM);
                    mSM.SwitchState(mSM.WalkState);
                }
                else if(mSM.calculatedCurVel < mSM.runLimit){
                    SlideToMoveState(mSM);
                    mSM.SwitchState(mSM.JogState);
                }
                else{
                    SlideToMoveState(mSM);
                    mSM.SwitchState(mSM.RunState);
                }
        }
        */
        mSM.SlideMovement();
    }
}
