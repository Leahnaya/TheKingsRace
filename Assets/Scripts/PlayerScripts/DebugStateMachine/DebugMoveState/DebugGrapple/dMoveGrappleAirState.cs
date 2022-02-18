using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveGrappleAirState : dMoveBaseState
{
    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){
        mSM.driftVel = Vector3.zero; // clears driftVel
    }

    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

    }

    public override void UpdateState(dMoveStateManager mSM){

        //checks if aerial state manager is no longer air grappling
        if(mSM.aSM.currentState != mSM.aSM.GrappleAirState){
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
        //Directional movement to prevent weird movement issue
        mSM.DirectionalMovement();
    }

}
