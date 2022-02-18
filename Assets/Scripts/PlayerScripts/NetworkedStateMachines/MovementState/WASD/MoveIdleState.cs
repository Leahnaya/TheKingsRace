using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveIdleState : MoveBaseState
{
    public override void EnterState(MoveStateManager mSM, MoveBaseState previousState){
        
    }
    
    public override void ExitState(MoveStateManager mSM, MoveBaseState nextState){

    }

    public override void UpdateState(MoveStateManager mSM){

        //Move to Walk State after speed increases
        if(mSM.calculatedCurVel >= mSM.idleLimit){
            mSM.SwitchState(mSM.WalkState);
        }

        //If Q or joystick button1 crouch state
        else if((Input.GetKey(KeyCode.JoystickButton1) || Input.GetKey(KeyCode.Q)) && (mSM.aSM.currentState != mSM.aSM.FallingState && mSM.aSM.currentState != mSM.aSM.WallRunState && mSM.aSM.currentState != mSM.aSM.WallIdleState && mSM.aSM.currentState != mSM.aSM.GrappleGroundedState)){
            mSM.SwitchState(mSM.CrouchState);
        }
    }

    public override void FixedUpdateState(MoveStateManager mSM){
        
        //actual directional movement
        mSM.DirectionalMovement();
    }
}
