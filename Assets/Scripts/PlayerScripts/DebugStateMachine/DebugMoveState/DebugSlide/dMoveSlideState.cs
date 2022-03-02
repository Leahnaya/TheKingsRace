using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveSlideState : dMoveBaseState
{
    //Slide Variables
    RaycastHit slideRay; // slide raycast

    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){

        //Rotate player and adjust height and adjust traction
        mSM.pStats.CurVel = 0;
        mSM.moveController.height *= .5f;
        mSM.pStats.CurTraction = 0.01f;
        mSM.moveController.center = new Vector3(0,mSM.moveController.center.y - mSM.moveController.height * .5f,0);
        //mSM.moveController.Move(new Vector3(0,-0.1f,0));
    }
    
    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

        //if next state isn't crouch then revert player rotation, height, and traction
        if(nextState != mSM.CrouchState){
            mSM.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
            mSM.pStats.CurVel = mSM.calculatedCurVel;
            mSM.pStats.CurTraction = mSM.pStats.Traction;
            mSM.moveController.height *= 2.0f;
            mSM.moveController.center = new Vector3(0,mSM.moveController.center.y + mSM.moveController.height * .25f,0);
        }

        //if state is crouch revert traction
        else{
            mSM.pStats.CurTraction = mSM.pStats.Traction;
        }
    }

    public override void UpdateState(dMoveStateManager mSM){

        //if player comes to a stop while sliding they crouch
        if(mSM.calculatedCurVel < mSM.idleLimit){
            mSM.SwitchState(mSM.CrouchState);
        }
        
        //steadily increase traction
        mSM.pStats.CurTraction += .004f;
        
        //If player isn't pressing either Q or the joystick button they stop sliding if nothing is above them
        if((!Input.GetKey(KeyCode.JoystickButton1) && !Input.GetKey(KeyCode.Q))){
            if ((Physics.Raycast(mSM.gameObject.transform.position + new Vector3(0,1f,0), Vector3.up, out slideRay, 2f, mSM.layerMask) == false)){

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
                Debug.Log(slideRay.collider.name);
            }

        }

        //Debug.DrawRay(mSM.gameObject.transform.position + new Vector3(0,1f,0), Vector3.up * 2f, Color.red);

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

        //counter rotates player so they don't rotate when camera is turned
        mSM.transform.Rotate(Vector3.up * -mSM.sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));

        //actual slide movement
        mSM.SlideMovement();
    }
}
