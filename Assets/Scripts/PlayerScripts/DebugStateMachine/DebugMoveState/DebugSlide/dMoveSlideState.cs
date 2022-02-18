using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveSlideState : dMoveBaseState
{
    //Slide Variables
    float originalTraction; // Traction before slide started
    RaycastHit slideRay; // slide raycast

    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){

        //Rotate player and adjust height and adjust traction
        mSM.pStats.CurVel = 0;
        originalTraction = mSM.pStats.Traction;
        mSM.gameObject.transform.eulerAngles = new Vector3(mSM.transform.eulerAngles.x - 90, mSM.transform.eulerAngles.y, mSM.transform.eulerAngles.z);
        mSM.moveController.height *= .5f;
        mSM.pStats.Traction = 0.01f;
    }
    
    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

        //if next state isn't crouch then revert player rotation, height, and traction
        if(nextState != mSM.CrouchState){
            mSM.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            mSM.pStats.CurVel = mSM.calculatedCurVel;
            mSM.pStats.Traction = originalTraction;
            mSM.moveController.height *= 2.0f;
        }

        //if state is crouch revert traction
        else{
            mSM.pStats.Traction = originalTraction;
        }
    }

    public override void UpdateState(dMoveStateManager mSM){

        //if player comes to a stop while sliding they crouch
        if(mSM.calculatedCurVel < mSM.idleLimit){
            mSM.SwitchState(mSM.CrouchState);
        }
        
    }

    public override void FixedUpdateState(dMoveStateManager mSM){

        //counter rotates player so they don't rotate when camera is turned
        mSM.transform.Rotate(Vector3.forward * -mSM.sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        //steadily increase traction
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

        //actual slide movement
        mSM.SlideMovement();
    }
}
