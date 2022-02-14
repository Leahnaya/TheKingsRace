using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCrouchState : MoveBaseState
{

    //Slide Variables
    float originalTraction; // Traction before slide started
    RaycastHit slideRay; // slide raycast

    public override void EnterState(MoveStateManager mSM){
        Debug.Log("Crouch State");

        if(mSM.previousState != mSM.SlideState){
            //Initialize Important Stats On state enter
            mSM.pStats.CurVel = 0;
            originalTraction = mSM.pStats.Traction;
            mSM.gameObject.transform.eulerAngles = new Vector3(mSM.transform.localEulerAngles.x - 90, mSM.transform.localEulerAngles.y, mSM.transform.localEulerAngles.z);
            mSM.moveController.height *= .5f;
            mSM.pStats.Traction = 0.01f;
        }
    }
    
    public override void UpdateState(MoveStateManager mSM){
        
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        mSM.transform.Rotate(Vector3.forward * -mSM.sensitivity * Time.deltaTime * Input.GetAxis("Mouse X"));
        mSM.pStats.Traction += .004f;

        ///////ONCE WE HAVE IT SO SLIDE DOESNT ROTATE PLAYER MOVE THIS TO UPDATE
        //If player isn't pressing either Q or the joystick button they stop crouching if nothing is above them
        if((!Input.GetKey(KeyCode.JoystickButton1) && !Input.GetKey(KeyCode.Q))){
            if ((Physics.Raycast(mSM.gameObject.transform.position, mSM.slideUp, out slideRay, 5f) == false)){

                ExitCrouchState(mSM);
                mSM.SwitchState(mSM.IdleState);
            }
            else{
                Debug.Log("Object above you");
            }
        }

        mSM.SlideMovement();
    }

    public override void OnCollisionEnter(MoveStateManager mSM){

    }

    public void ExitCrouchState(MoveStateManager mSM){
        mSM.gameObject.transform.localEulerAngles = new Vector3(0, 0, 0);
        mSM.pStats.CurVel = mSM.calculatedCurVel;
        mSM.pStats.Traction = originalTraction;
        mSM.moveController.height *= 2.0f;
    }
}
