using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dMoveCrouchWalkState : dMoveBaseState
{
    public override void EnterState(dMoveStateManager mSM, dMoveBaseState previousState){

    }

    public override void ExitState(dMoveStateManager mSM, dMoveBaseState nextState){

    }
    
    public override void UpdateState(dMoveStateManager mSM){

    }

    public override void FixedUpdateState(dMoveStateManager mSM){

        /*
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
    }

}
